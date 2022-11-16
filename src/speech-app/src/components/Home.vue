<template>
    <h1>Speech Translation</h1>
      <button @click="host">Host</button> 
      <button @click="consumer">Join</button> 
  </template>

<script>
export default {
  name: 'HomeComp',
  methods:{
    host(){
      this.$router.push({path:'host'})
    },
    consumer(){
      this.$router.push({path:'consumer'})
    },
  }
}

</script>

<style scoped>

button {
  width: 100px;
  height: 50px;
  margin-left: 10px;
}
</style>