<template>
  <nav>
    <!-- Sadece giriş yapılmışsa çıkış butonunu göster -->
    <button v-if="isLoggedIn" @click="logout">Çıkış Yap</button>
  </nav>
  <router-view />
</template>

<script>
  export default {
    data() {
      return {
        isLoggedIn: false // Başlangıçta oturum açılmamış
      };
    },
    mounted() {
      // Sayfa yüklendiğinde token kontrolü yap
      this.checkLoginStatus();
    },
    methods: {
      checkLoginStatus() {
        // Token kontrolü yaparak isLoggedIn durumunu ayarla
        this.isLoggedIn = localStorage.getItem('token') !== null;
      },
      logout() {
        // Logout işlemi ve yönlendirme
        localStorage.removeItem('token');
        this.isLoggedIn = false; // Logout olduktan sonra butonun kaybolmasını sağla
        this.$router.push('/'); // Ana sayfaya yönlendir
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
