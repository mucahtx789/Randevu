<template>
  <div class="container">
    <h1>Admin Girişi</h1>
    <input v-model="tcNo" maxlength="11" placeholder="TC Kimlik No" />
    <input v-model="password" type="password" placeholder="Şifre" />
    <button @click="login">Giriş Yap</button>

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
      };
    },
    methods: {
      // Giriş işlemi
      async login() {
        try {
          const response = await axios.post(
            'http://localhost:5229/api/auth/login',  // API URL'sini doğru girdiğinizden emin olun
            {
              TcNo: this.tcNo,
              Password: this.password,
              Role: 'Admin'  // Sabit olarak Admin rolünü gönderiyoruz
            }
          );

          console.log('Login successful', response.data); // API'den dönen yanıtı kontrol edin

          // Kullanıcı rolüne göre işlem yapılıyor
          if (response.data.role === 'Admin') {
            localStorage.setItem('token', response.data.token);  // Token'ı localStorage'a kaydediyoruz
            localStorage.setItem('role', response.data.role);  // role bilgisini ekliyoruz
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            this.$router.push('/doctor-list');  // Başka bir sayfaya yönlendirme yapıyoruz
          } else {
            alert('Yetkisiz giriş!');
          }
        } catch (error) {
          console.error(error);  // Detaylı hata bilgisi konsola yazdırılacak
          if (error.response) {
            alert(error.response.data.message || 'Giriş Başarısız');
          } else {
            alert('Giriş Başarısız');
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
