namespace SpeechTranslation;
using Microsoft.Extensions.Configuration;

public class SpeechConfigParam{

    IConfigurationBuilder _builder;
    IConfigurationRoot _configuration;

    string _configKey;
    string _location;

    public string ConfigKey{
        get {
            return _configKey;
        }
    }

    public  string Location{
        get{
            return _location;
        }   
    }

    public SpeechConfigParam(){
        _builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

        _configuration = _builder.Build();
        _configKey = _configuration["CognitiveServiceKey"];
        _location = _configuration["CognitiveServiceRegion"];
    }
}