namespace SpeechTranslation;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

public class SpeechToText: ITranslate {

    private static SpeechConfig _speechConfig;

    private static SpeechConfigParam _configParam;

   public SpeechToText(SpeechConfig speechConfig){
        _speechConfig = speechConfig;
    }

    public async Task TranslateAsync(){
        try{
            Console.WriteLine("Ready to use speech service in " + _speechConfig.Region);

            // Configure voice
            _speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";

            await ContinuousRecognitionAsync();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static async Task ContinuousRecognitionAsync()
    {
        using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
        {   
            using (var recognizer = new  SpeechRecognizer(_speechConfig, audioInput)){
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

    static async Task RecongnizeOnceAsync(){
        string command = "";
        
        // Configure speech recognition
        using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using SpeechRecognizer speechRecognizer = new SpeechRecognizer(_speechConfig, audioConfig);

        Console.WriteLine("Speak now...");

        // Process speech input
        SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();
        if (speech.Reason == ResultReason.RecognizedSpeech)
        {
            command = speech.Text;
            Console.WriteLine(command);
        }
        else
        {
            Console.WriteLine(speech.Reason);
            if (speech.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(speech);
                Console.WriteLine(cancellation.Reason);
                Console.WriteLine(cancellation.ErrorDetails);
            }
        }       

    }

    static async Task TellTime(){
        var now = DateTime.Now;
        string responseText = "The time is " + now.Hour.ToString() + ":" + now.Minute.ToString("D2");
                    
        // Configure speech synthesis
        _speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
        using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(_speechConfig);

        /*speechConfig.SpeechSynthesisVoiceName = "en-GB-LibbyNeural"; // change this
        using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);*/

        //ssml
        /*string responseSsml = $@"
            <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                <voice name='en-GB-LibbyNeural'>
                    {responseText}
                    <break strength='weak'/>
                    Time to end this lab!
                </voice>
            </speak>";
        SpeechSynthesisResult speak = await speechSynthesizer.SpeakSsmlAsync(responseSsml);
        if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine(speak.Reason);
        }*/

        // Synthesize spoken output
        SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
        if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine(speak.Reason);
        }

        // Print the response
        Console.WriteLine(responseText);
    }
}