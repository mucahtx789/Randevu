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
    padding: 20px;
    background: rgba(255, 255, 255, 0.9);
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    font-family: "Segoe UI", sans-serif;
    text-align: center;
  }

  h2 {
    margin-bottom: 20px;
    color: #2c3e50;
  }

  form > div {
    margin-bottom: 15px;
    text-align: left;
  }

  label {
    display: block;
    margin-bottom: 6px;
    font-weight: 600;
    color: #34495e;
  }

  input {
    width: 100%;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 14px;
  }

  button {
    display: inline-block;
    background-color: #007bff;
    color: white;
    border: none;
    padding: 10px 16px;
    font-size: 14px;
    border-radius: 4px;
    cursor: pointer;
    margin-top: 10px;
    transition: background-color 0.3s ease;
  }

    button:hover {
      background-color: #0056b3;
    }

  router-link button {
    background-color: #6c757d;
  }

    router-link button:hover {
      background-color: #5a6268;
    }
  button[type="submit"] {
    display: inline-block;
    background-color: #28a745; /* yeşil */
    color: #000; /* siyah yazı */
    border: none;
    padding: 10px 16px;
    font-size: 14px;
    border-radius: 4px;
    cursor: pointer;
    margin-top: 10px;
    transition: background-color 0.3s ease;
  }

    button[type="submit"]:hover {
      background-color: #218838; /* daha koyu yeşil */
    }
</style>
