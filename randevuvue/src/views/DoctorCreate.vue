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
    margin: auto;
    padding: 20px;
    background: rgba(255, 255, 255, 0.95);
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    font-family: "Segoe UI", sans-serif;
    text-align: center;
  }

  h1 {
    margin-bottom: 20px;
    color: #2c3e50;
  }

  form {
    display: flex;
    flex-direction: column;
    text-align: left;
  }

  label {
    margin-top: 1rem;
    font-weight: 600;
    color: #34495e;
  }

  input,
  select {
    padding: 0.5rem;
    font-size: 1rem;
    margin-top: 0.25rem;
    border: 1px solid #ccc;
    border-radius: 4px;
  }

  button[type="submit"] {
    background-color: #28a745; /* yeşil */
    color: #000; /* siyah yazı */
    margin-top: 20px;
    padding: 10px 16px;
    font-size: 14px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }

    button[type="submit"]:hover {
      background-color: #218838;
    }

  button:not([type="submit"]) {
    background-color: #6c757d;
    color: white;
    margin-top: 10px;
    padding: 10px 16px;
    font-size: 14px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }

    button:not([type="submit"]):hover {
      background-color: #5a6268;
    }
</style>
