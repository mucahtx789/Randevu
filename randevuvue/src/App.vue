<template>
  <nav>
    <button v-if="isLoggedIn" @click="logout">Çıkış Yap</button>
  </nav>
  <router-view />
</template>

<script>
  export default {
    computed: {
      isLoggedIn() {
        return localStorage.getItem('token') !== null;
      }
    },
    methods: {
      logout() {
        localStorage.removeItem('token');
        this.$router.push('/');
      }
    }
  };
</script>

<style>
  nav {
    display: flex;
    justify-content: flex-end;
    padding: 10px;
  }
</style>
