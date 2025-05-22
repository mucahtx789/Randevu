<template>
  <div class="container">
    <h1>Admin Girişi</h1>
    <input v-model="tcNo" maxlength="11" placeholder="TC Kimlik No" />
    <input v-model="password" type="password" placeholder="Şifre" />

    <!-- vue3-recaptcha2 -->
    <div class="form-group">
      <vue-recaptcha ref="recaptcha"
                     :sitekey="siteKey"
                     @verify="onCaptchaResolved"
                     @expire="onCaptchaExpired"
                     @fail="onCaptchaError"
                     @error="onCaptchaError" />
    </div>
    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

    <button :disabled="!captchaToken" @click="login">Giriş Yap</button>

    <!-- Ana Sayfaya Git butonu her zaman görünür -->
    <button @click="goToHomePage">Hasta Girişi</button>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        tcNo: '',
        password: '',
        errorMessage: '',
        captchaToken: '',
        siteKey: '6Ld--S4rAAAAAFspcnjYmyIEPAopgaGh43l7r-Ni' // siteKey burada
      };
    },
    methods: {
      // reCAPTCHA doğrulama işlemi
      onCaptchaResolved(token) {
        this.captchaToken = token;
      },
      onCaptchaExpired() {
        this.captchaToken = ''; // Token süresi bittiğinde temizlenir
      },
      onCaptchaError() {
        this.errorMessage = 'reCAPTCHA doğrulaması başarısız oldu.'; // Eğer hata oluşursa kullanıcıya hata mesajı verilir
        this.captchaToken = ''; // Token temizlenir
      },
      // Giriş işlemi
      async login() {
        try {
          const response = await axios.post(
            'http://localhost:5229/api/auth/login',  
            {
              TcNo: this.tcNo,
              Password: this.password,
              Role: 'Admin',  // Sabit olarak Admin rolünü gönderiyoruz
              recaptchaToken: this.captchaToken 
            });

          console.log('Login successful', response.data); // API'den dönen yanıt

          // Kullanıcı rolüne göre işlem yapılıyor
          if (response.data.role === 'Admin') {
            const token = response.data.token;
            localStorage.setItem('token', response.data.token);  // Token'ı localStorage'a kaydediyoruz
            localStorage.setItem('role', response.data.role);  // role bilgisini ekliyoruz
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            this.$router.push('/doctor-list');  // Başka bir sayfaya yönlendirme
          } else {
            alert('Yetkisiz giriş!');
          }
        } catch (error) {
          console.error(error);  // Detaylı hata bilgisi konsola yazdırılacak
          if (error.response) {
            alert(error.response.data.message || 'Login failed');
          } else {
            alert('Login failed');
          }
        }
      },

      // Ana Sayfaya Yönlendirme
      goToHomePage() {
        // HomeView sayfasına yönlendir
        this.$router.push('/');
      }
    }
  };
</script>

<style scoped>
  .container {
    max-width: 400px;
    margin: 100px auto;
    padding: 30px;
    background-color: rgba(255, 255, 255, 0.95); /* yarı saydam beyaz kutu */
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  /* Başlık */
  h1 {
    margin-bottom: 30px;
    color: #1976d2; /* mavi tema rengi */
    font-size: 24px;
  }

  /* Inputlar */
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

  /* Ortak buton stili - App.vue ile bire bir aynı */
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
