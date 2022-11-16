import axios from 'axios'
//import constants from './constants'

export default {
  data() {
    return {
      toLanguages: []
    }
  },
  methods: {
    async getTargetLanguages() {
      const languages = (await axios.get(`http://localhost:7071/api/targetlanguages`)).data
      return {
        to: languages
      }
    }
  },
  async mounted() {
    const languages = await this.getTargetLanguages()
    this.toLanguages = languages.to
  }
}