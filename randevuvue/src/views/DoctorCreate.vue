<template>
  <div class="doctor-form">
    <h1>Yeni Doktor Kaydı</h1>
    <form @submit.prevent="createDoctor">

      <label>Ad</label>
      <input v-model="doctor.firstName" placeholder="Ad" required />

      <label>Soyad</label>
      <input v-model="doctor.lastName" placeholder="Soyad" required />

      <label>TC Kimlik No</label>
      <input v-model="doctor.tcNo" placeholder="TC Kimlik No" maxlength="11" required />

      <label>Şifre</label>
      <input v-model="doctor.password" type="password" placeholder="Şifre" required />

      <label>Branş</label>
      <select v-model="doctor.specialization" required>
        <option disabled value="">Branş Seçiniz</option>
        <option v-for="branch in specializations" :key="branch" :value="branch">
          {{ branch }}
        </option>
      </select>

      <label>Uzmanlık Seviyesi</label>
      <select v-model="doctor.experienceLevel" required>
        <option disabled value="">Uzmanlık Seçiniz</option>
        <option v-for="level in experienceLevels" :key="level" :value="level">
          {{ level }}
        </option>
      </select>

      <button type="submit">Kaydet</button>
      <button @click="goToDoctorList">Doktor Listesi</button>
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
          password: '',
          specialization: '',
          experienceLevel: ''
        },
        specializations: [
          'Çocuk Sağlığı ve Hastalıkları',
          'Dahiliye (İç Hastalıkları)',
          'Kadın Hastalıkları ve Doğum',
          'Kardiyoloji',
          'Kulak Burun Boğaz (KBB)',
          'Ortopedi ve Travmatoloji',
          'Göz Hastalıkları',
          'Nöroloji',
          'Psikiyatri',
          'Cildiye (Dermatoloji)',
          'Göğüs Hastalıkları',
          'Üroloji',
          'Genel Cerrahi',
          'Beyin ve Sinir Cerrahisi',
          'Fizik Tedavi ve Rehabilitasyon',
          'Enfeksiyon Hastalıkları',
          'Anesteziyoloji ve Reanimasyon'
        ],
        experienceLevels: [
          'Pratisyen Hekim',
          'Uzman Doktor',
          'Doçent Doktor',
          'Profesör Doktor'
        ]
      };
    },
    methods: {
      async createDoctor() {
        try {
          await axios.post('http://localhost:5229/api/admin/doctors/create', this.doctor);
          alert('Doktor başarıyla eklendi!');
          this.$router.push('/doctor-list');
        } catch (error) {
          alert('Hata: ' + (error.response?.data?.message || error.message));
        }        
      },
      goToDoctorList() {
        this.$router.push('/doctor-list');
      }
    }
  };
</script>

<style scoped>
  .doctor-form {
    max-width: 400px;
    margin: 0 auto;
    padding: 2rem;
  }

  form {
    display: flex;
    flex-direction: column;
  }

  label {
    margin-top: 1rem;
    font-weight: bold;
  }

  input,
  select {
    padding: 0.5rem;
    font-size: 1rem;
    margin-top: 0.25rem;
  }

  button {
    margin-top: 2rem;
    padding: 0.75rem;
    font-size: 1rem;
    background-color: #2e86de;
    color: white;
    border: none;
    cursor: pointer;
  }

    button:hover {
      background-color: #1e5fa9;
    }
</style>
