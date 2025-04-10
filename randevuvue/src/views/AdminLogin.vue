<template>
  <div class="container">
    <h1>Giriş Yap</h1>
    <input v-model="tcNo" placeholder="TC Kimlik No" />
    <input v-model="password" type="password" placeholder="Şifre" />
    <button @click="login">Giriş Yap</button>
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
      }
    }
  };
</script>
