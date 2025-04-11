<template>
  <div class="container">
    <h1>Hasta Girişi</h1>
    <input v-model="tcNo" placeholder="TC Kimlik No" maxlength="11" />
    <input v-model="password" type="password" placeholder="Şifre" />
    <button @click="login">Giriş Yap</button>

    <button @click="goToAdminLogin">Admin Girişi</button>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      tcNo: '',
      password: ''
    };
  },
  methods: {
    async login() {
      try {
        const response = await axios.post('http://localhost:5295/api/auth/login', {
          TCNo: this.tcNo,
          Password: this.password
        });
        localStorage.setItem('token', response.data.token);
        this.$router.push('/dashboard');
      } catch (error) {
        alert('Giriş Başarısız');
      }
    },
    goToAdminLogin() {
      this.$router.push('/admin-login');
    }
  }
};
</script>

<style>
  .container {
    max-width: 300px;
    margin: auto;
    text-align: center;
  }
</style>
