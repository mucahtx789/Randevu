<template>
  <div class="container">
    <h1>Hasta Girişi</h1>
    <input v-model="tcNo" placeholder="TC Kimlik No" maxlength="11" />
    <input v-model="password" type="password" placeholder="Şifre" />
    <button @click="login">Giriş Yap</button>
    <button @click="goToRegister">Hasta Kayıt </button>
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

<style scoped>
  /* Hasta Girişi sayfası için container */
  .container {
    max-width: 400px;
    margin: 100px auto;
    padding: 30px;
    background-color: rgba(255, 255, 255, 0.95); /* Yarı saydam beyaz */
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  /* Başlık */
  h1 {
    margin-bottom: 30px;
    color: #1976d2; /* Mavi renk */
    font-size: 24px;
  }

  /* Input kutuları */
  input {
    display: block;
    width: 100%;
    padding: 10px 12px;
    margin-bottom: 15px;
    font-size: 16px;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-sizing: border-box;
  }

  /* Buton stili (Admin Girişi ile birebir aynı) */
  button {
    background-color: #2196f3;
    color: white;
    border: none;
    padding: 10px 18px;
    font-size: 16px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    margin: 5px;
    width: 100%;
  }

    button:hover {
      background-color: #1976d2;
    }
</style>
