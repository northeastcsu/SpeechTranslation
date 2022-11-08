using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;

namespace SpeechTranslation;

public static class SpeechConfigBuilder{

    private static SpeechConfig? _speechConfig;
    public static SpeechConfig Build(){
        
        if(_speechConfig == null){
             var configurationBuilder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
        
            var configuration = configurationBuilder.Build();

            _speechConfig = SpeechConfig.FromSubscription(configuration["CognitiveServiceKey"], configuration["CognitiveServiceRegion"]);
        
        }

       return _speechConfig;
    }

}