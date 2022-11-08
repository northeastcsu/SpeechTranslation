namespace SpeechTranslation;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System.Threading.Tasks;
using System.Text;

public class SpeechToSpeech : ITranslate {

    private static SpeechConfig _speechConfig;
    private static SpeechTranslationConfig translationConfig;


    public SpeechToSpeech(SpeechConfig speechConfig){
        _speechConfig = speechConfig;
    }

    public async Task TranslateAsync(){
        try{
            
            // Configure translation
            translationConfig = SpeechTranslationConfig.FromSubscription(_speechConfig.SubscriptionKey,_speechConfig.Region);
            translationConfig.SpeechRecognitionLanguage = "en-US";
            translationConfig.AddTargetLanguage("fr");
            translationConfig.AddTargetLanguage("es");
            translationConfig.AddTargetLanguage("hi");
            Console.WriteLine("Ready to translate from " + translationConfig.SpeechRecognitionLanguage);

            string targetLanguage = "";
            while (targetLanguage != "quit")
            {
                Console.WriteLine("\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n");
                targetLanguage=Console.ReadLine().ToLower();
                if (translationConfig.TargetLanguages.Contains(targetLanguage))
                {
                    await Translate(targetLanguage);
                }
                else
                {
                    targetLanguage = "quit";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static async Task Translate(string targetLanguage){
        string translation = "";

        // Translate speech
        using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using TranslationRecognizer translator = new TranslationRecognizer(translationConfig, audioConfig);
        Console.WriteLine("Speak now...");
        TranslationRecognitionResult result = await translator.RecognizeOnceAsync();
        Console.WriteLine($"Translating '{result.Text}'");
        translation = result.Translations[targetLanguage];
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine(translation);

        // Synthesize translation
        var voices = new Dictionary<string, string>
            {
                ["fr"] = "fr-FR-HenriNeural",
                ["es"] = "es-ES-ElviraNeural",
                ["hi"] = "hi-IN-MadhurNeural"
            };
        _speechConfig.SpeechSynthesisVoiceName = voices[targetLanguage];
        using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(_speechConfig);
        SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(translation);
        if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine(speak.Reason);
        }

    }

}