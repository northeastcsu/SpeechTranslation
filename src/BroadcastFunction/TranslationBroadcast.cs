using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using BroadcastFunction.Utils;
using System.Linq;

namespace BroadcastFunction
{
    public static class TranslationBroadcast
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "POST", Route = "{userId}/negotiate")]HttpRequest req,
            [SignalRConnectionInfo(HubName = "captions", UserId = "{userId}")]SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
        
        [FunctionName(nameof(TargetLanguages))]
        public static IActionResult TargetLanguages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET")] HttpRequest req)
        {
            return new OkObjectResult(Constants.LANGUAGES);
        }


        [FunctionName(nameof(SelectLanguage))]
        public static async Task SelectLanguage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] dynamic payload,
            [SignalR(HubName = "captions")] IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            var languageCode = payload.languageCode.ToString();

            var groupActionsTasks = 
                Constants.LANGUAGE_CODES.Select(lc => 
                    signalRGroupActions.AddAsync(new SignalRGroupAction
                    {
                        UserId = payload.userId,
                        GroupName = lc,
                        Action = lc == languageCode ? GroupAction.Add : GroupAction.Remove
                    }));

            await Task.WhenAll(groupActionsTasks);
        }


        [FunctionName("TranslationBroadcast")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Post", Route = null)] HttpRequest req,
            [SignalR(HubName = "captions")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var captionTasks = new List<Task>();
            var translations = data.languages.ToObject<Dictionary<string, string>>();

            Console.WriteLine(data);
            foreach (var translation in translations)
            {
                string value = translation.Value;
                log.LogWarning(value);

                var caption = new
                {
                    language = translation.Key,
                    offset = data.offset,
                    text = translation.Value
                };

                captionTasks.Add(signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "newCaption",
                    GroupName = translation.Key,
                    Arguments = new[] { caption }
                }));
            }

            await Task.WhenAll(captionTasks);
        }
    }
}
