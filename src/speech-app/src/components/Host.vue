<template>
  <h1>Host</h1>
  <button @click="home">Home</button> 
  <div class="caption-host">
    <div v-if="!started">
      <h1>Host</h1>
      <div><input id="speechkey" v-model="key" placeholder="Cognitive Services Speech API Key" /></div>
      <div>
        <!---<select v-model="fromLanguage">
          <option v-for="lang in fromLanguages" :value="lang" :key="lang">
            {{ lang }}
          </option>
        </select>-->
        <p><button @click="start">start</button></p>
      </div>
    </div>
    <div v-else>
      <p><button @click="stop">stop</button></p>
      <div id="currentSentence" class="caption">
        {{ currentSentence }}
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import constants from '../lib/constants'
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
      fromLanguage: ["en-US"]
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
      axios.post(`${constants.apiBaseUrl}/api/TranslationBroadcast`,
        captions.translations)
    }.bind(this))
    
  },
  methods: {
    start() {
      console.log("to languages")
     
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
    },
    home() {
      console.log('here')
      this.$router.push({path:'/'})
    }
  },
  beforeUnmount() {
    this.translator.stop()
  },
  
}
</script>

<style scoped>
input[id=speechkey] {
  width: 600px;
  height: 40px;
}

button {
  width: 100px;
  height: 50px;
  margin-left: 10px;
}

</style>

<style scoped>
input[id=speechkey] {
  width: 600px;
  height: 40px;
}

button {
  width: 100px;
  height: 50px;
  margin-left: 10px;
}

</style>