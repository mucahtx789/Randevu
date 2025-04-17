<template>
  <div class="register-container">
    <h2>Hasta Kayıt</h2>
    <form @submit.prevent="register">
      <div>
        <label>TC Kimlik No:</label>
        <input v-model="tcNo" maxlength="11" required />
      </div>
      <div>
        <label>Ad Soyad:</label>
        <input v-model="fullName" required />
      </div>
      <div>
        <label>Şifre:</label>
        <input type="password" v-model="password" required />
      </div>
      <div>
        <label>Doğum Tarihi:</label>
        <input type="date" v-model="birthDate" required />
      </div>
      <button type="submit">Kayıt Ol</button>
    </form>

    <br />

    <router-link to="/">
      <button>Anasayfaya Dön</button>
    </router-link>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      tcNo: '',
      fullName: '',
      password: '',
      birthDate: ''
    };
  },
  methods: {
    async register() {
      try {
        const response = await axios.post('http://localhost:5229/api/patients/register', {
          tcNo: this.tcNo,
          fullName: this.fullName,
          password: this.password,
          birthDate: this.birthDate
        });

        if (response.status === 200) {
          alert('Kayıt başarılı!');
          this.$router.push('/');
        }
      } catch (error) {
        console.error(error);
        alert('Kayıt sırasında hata oluştu.');
      }
    }
  }
};
</script>

<style scoped>
  .register-container {
    max-width: 400px;
    margin: auto;
  }
</style>
