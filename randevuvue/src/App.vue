<template>
  <nav>
    <!-- Giriş yapılmışsa çıkış butonunu göster -->
    <button v-if="isLoggedIn" @click="logout" class="logout-btn">Çıkış Yap</button>
  </nav>
  <router-view />
</template>

<script>
  export default {
    data() {
      return {
        isLoggedIn: false
      };
    },
    created() {
      this.checkLoginStatus(); // Sayfa ilk yüklendiğinde kontrol
    },
    mounted() {
      // Sayfa geçişlerinden sonra login durumu kontrol edilir
      this.$router.afterEach(() => {
        this.checkLoginStatus();
      });
    },
    methods: {
      checkLoginStatus() {
        this.isLoggedIn = localStorage.getItem('token') !== null;
      },
      logout() {
        localStorage.removeItem('token');
        this.isLoggedIn = false;
        this.$router.push('/');
      }
    }
  };
</script>

<style>
  /* NAVIGATION */
  nav {
    display: flex;
    justify-content: flex-end;
    background-color: #e3f2fd;
    padding: 15px 30px;
    border-bottom: 2px solid #bbdefb;
  }

  /* BUTON */
  .logout-btn {
    background-color: #2196f3;
    color: white;
    border: none;
    padding: 10px 18px;
    font-size: 16px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }

    .logout-btn:hover {
      background-color: #1976d2;
    }

  /* GENEL TEMA */
  body {
    margin: 0;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-image: url('/images/hastane-arka-plan.jpg');
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
    background-attachment: fixed;
  }
</style>
