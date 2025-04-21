<template>
  <div class="container">
    <h1>Hasta Girişi</h1>
    <input v-model="tcNo" placeholder="TC Kimlik No" maxlength="11" />
    <input v-model="password" type="password" placeholder="Şifre" />
    <button @click="login">Giriş Yap</button>

    <button @click="goToAdminLogin">Admin Girişi</button>
    <button @click="goToRegister">Kayıt Ol</button>
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
          const response = await axios.post('http://localhost:5229/api/auth/login', {
            TcNo: this.tcNo,
            Password: this.password,
            Role: 'Patient' // Hasta girişi için rol bilgisi gönderiyoruz
          });

          // Kontrol: gelen rol gerçekten Patient mi?
          if (response.data.role === 'Patient') {
            const token = response.data.token;
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('role', response.data.role);  // role bilgisini ekliyoruz
            localStorage.setItem('patientId', response.data.patientId);
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

            this.$router.push('/dashboard'); // Hasta paneline yönlendirme
          } else {
            alert('Yetkisiz giriş!');
          }
        } catch (error) {
          console.error(error);
          if (error.response) {
            alert(error.response.data.message || 'Giriş Başarısız');
          } else {
            alert('Giriş Başarısız');
          }
        }
      },
      goToAdminLogin() {
        this.$router.push('/admin-login');
      },
      goToRegister() {
        this.$router.push('/patient-register');
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

  button {
    margin-top: 10px;
  }
</style>
