<template>
    <div class="caption-host">
    <div v-if="!started">
      <h1>Host</h1>
      <div><input v-model="key" placeholder="Cognitive Services Speech API Key" /></div>
      <div>
        <select v-model="fromLanguage">
          <option v-for="lang in fromLanguages" :value="lang" :key="lang">
            {{ lang }}
          </option>
      
        </select>
        <button @click="start">start</button>
      </div>
    </div>
    <div v-else>
      <button @click="stop">stop</button>
      <div id="currentSentence" class="caption">
        {{ currentSentence }}
      </div>
    </div>
  </div>
</template>

<script>
//import axios from 'axios'
//import constants from '../lib/constants'
import Translator from '../lib/translator'
import languages from '../lib/language'

const speechApiKeyLocalStorageKey = 'speechApiKey'

export default {
  name:"HostComp",
  mixins: [ languages ],
  data() {
    return {
      key: window.localStorage.getItem(speechApiKeyLocalStorageKey) || '',
      region: 'eastus',
      currentSentence: '',
      started: false,
      fromLanguage: ["en-US"],
      toLanguage: ["en-US"],
      translations:''
    }
  },
  watch: {
    key(newKey) {
      window.localStorage.setItem(speechApiKeyLocalStorageKey, newKey)
    }
  },
  created() {
    this.translator = new Translator(function(captions) {
      console.log(captions)
      this.currentSentence += captions.original
      //axios.post(`${constants.apiBaseUrl}/api/captions`,
      //  captions.translations,
      //  { withCredentials: true })
    }.bind(this))
    
  },
  methods: {
    start() {
      console.log("here1")
      console.log(this.toLanguage)
      this.translator.start({
        key: this.key,
        region: this.region,
        fromLanguage: "en-US",
        toLanguages: this.toLanguages
      })
      this.started = true
    },
    stop() {
      this.started = false
      this.currentSentence = ''
      this.translator.stop()
    }
  },
  beforeUnmount() {
    this.translator.stop()
  }
}
</script>

<style scoped>
input[type=password] {
  width: 600px;
}
</style>