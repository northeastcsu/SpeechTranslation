using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace SpeechTranslation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string option = "";
            ITranslate? translate = null;

            translate = new SpeechToText(SpeechConfigBuilder.Build());
            await translate.TranslateAsync();

            /*while(option != "3"){
                Console.WriteLine("Enter translation option:");
                Console.WriteLine("1. Speech to text");
                Console.WriteLine("2. Speech to speech");
                Console.WriteLine("3. Quit");

                option = Console.ReadLine();

                switch(option){
                    case "1" : {
                        translate = new SpeechToText(SpeechConfigBuilder.Build());
                        break;
                    }
                    case "2":{
                        translate = new SpeechToSpeech(SpeechConfigBuilder.Build());
                        break;
                    }
                    default:{
                        translate = null;
                        break;
                    }
                }

                if(translate != null){
                    await translate.TranslateAsync();
                }
            }*/
        }

        private static async Task ContinousSpeech(){
            var config = SpeechConfig.FromSubscription("8fa3f0fddc6a47eeac0427c0ae795c22","eastus");

            //var stopRecognition = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
            {   
                using (var recognizer = new  SpeechRecognizer(config, audioInput)){
                   /* recognizer.Recognizing += (s, e) => {
                        Console.WriteLine($"RECOGNIZING: {e.Result.Text}");
                    };*/

                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"{e.Result.Text}");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\n    Session started event.");
                    };
                    
                    recognizer.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\n    Session ended.");
                    };
                    
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    bool process = true;
                    do { 
                        Console.WriteLine("Press 2 to stop");
                        var option = Console.ReadLine();
                        if(option == "2"){
                            process = false;
                        }
                    }while(process);

                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
        }
    }
}