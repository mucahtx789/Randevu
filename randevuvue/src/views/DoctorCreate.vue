<template>
  <div>
    <h1>Yeni Doktor Kaydı</h1>
    <form @submit.prevent="createDoctor">
      <input v-model="doctor.firstName" placeholder="Ad" required />
      <input v-model="doctor.lastName" placeholder="Soyad" required />
      <input v-model="doctor.tcNo" placeholder="TC Kimlik No" required />
      <input v-model="doctor.password" type="password" placeholder="Şifre" required />
      <button type="submit">Kaydet</button>
    </form>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      doctor: {
        firstName: '',
        lastName: '',
        tcNo: '',
        password: ''
      }
    };
  },
  methods: {
    async createDoctor() {
      try {
        await axios.post('http://localhost:5229/api/admin/doctors/create', this.doctor);
        this.$router.push('/doctor-list');
      } catch (error) {
        alert('Hata: ' + error.response.data);
      }
    }
  }
};
</script>
