<template>
   <div id="nav">
      
  </div>
  <router-view />
</template>

<script>
import { v4 as uuidv4 } from 'uuid';
import constants from './lib/constants'

export default {
  data() {
    return {
      clientId: uuidv4(),
      title: constants.title
    }
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
