import { 
    AudioConfig, SpeechTranslationConfig, TranslationRecognizer, ResultReason 
  } from 'microsoft-cognitiveservices-speech-sdk'
  
  class Translator {
    _recognizer
    _callback
  
    constructor(callback) {
      this._callback = callback
    }
  
    start(options) {
      console.log('started')
      const alreadyStarted = !!this._recognizer
      console.log(alreadyStarted)
      if (alreadyStarted) {
        return
      }
      console.log("here")
      const audioConfig = AudioConfig.fromDefaultMicrophoneInput()
      const speechConfig = SpeechTranslationConfig.fromSubscription(options.key, options.region)
  
      speechConfig.speechRecognitionLanguage = options.fromLanguage
      //speechConfig.speechRecognitionLanguage = "en-us"
      for (const lang of options.toLanguages) {
        speechConfig.addTargetLanguage(lang)
      }
      console.log(speechConfig)
  
      this._recognizer = new TranslationRecognizer(speechConfig, audioConfig)
      //this._recognizer.recognizing = this._recognizer.recognized = recognizerCallback.bind(this)
      //this._recognizer.recognizing  = recognizerCallback.bind(this)
      this._recognizer.recognized = recognizerCallback.bind(this)
      this._recognizer.startContinuousRecognitionAsync()
      
      function recognizerCallback(s, e) {
        const result = e.result
        const reason = ResultReason[result.reason]
        if (reason !== 'TranslatingSpeech' && reason !== 'TranslatedSpeech') {
          return
        }
        console.log(result)
        const captions = {
          offset: result.offset,
          languages: {}
        }
        captions.languages[getLanguageCode(options.fromLanguage)] = result.text
        
  
        for (const lang of options.toLanguages) {
          const langCode = getLanguageCode(lang)
          const caption = result.translations.get(langCode)
          captions.languages[langCode] = caption
        }
  
        this._callback({
          original: result.text,
          translations: captions
        })
      }
  
      function getLanguageCode(lang) {
        return lang.substring(0, 2)
      }
    }
    
    stop() {
      this._recognizer.stopContinuousRecognitionAsync(
        stopRecognizer.bind(this),
        function (err) {
          stopRecognizer().bind(this)
          console.error(err)
        }.bind(this)
      )
  
      function stopRecognizer() {
        this._recognizer.close()
        this._recognizer = undefined
        console.log('stopped')
      }
    }
  }
  
  export default Translator