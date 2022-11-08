<template>
    <div class="caption-host">
    <div v-if="!started">
      <h1>Host</h1>
      <div><input v-model="key" placeholder="Cognitive Services Speech API Key" /></div>
      <div>
        <select v-model="fromLanguage">
          <option v-for="lang in fromLanguage" :value="lang" :key="lang">
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
//mport languageListMixin from '../lib/language-list-mixin'

const speechApiKeyLocalStorageKey = 'speechApiKey'

export default {
  name:"HostComp",
  mixins: [ "en-us" ],
  data() {
    return {
      key: window.localStorage.getItem(speechApiKeyLocalStorageKey) || '',
      region: 'eastus',
      currentSentence: '',
      started: false,
      fromLanguage: ["en-us"]
    }
  },
  watch: {
    key(newKey) {
      window.localStorage.setItem(speechApiKeyLocalStorageKey, newKey)
    }
  },
  created() {
    this.translator = new Translator(function(captions) {
      this.currentSentence += captions.original
      //axios.post(`${constants.apiBaseUrl}/api/captions`,
      //  captions.translations,
      //  { withCredentials: true })
    }.bind(this))
  },
  methods: {
    start() {
      this.translator.start({
        key: this.key,
        region: this.region,
        fromLanguage: "en-us",
        toLanguages: "en-us"
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