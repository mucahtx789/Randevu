<template>
  <div>
    <h1>Doktor Listesi</h1>
    <button @click="goToCreateDoctor">Doktor Kaydı Oluştur</button>
    <table>
      <thead>
        <tr>
          <th>Doktor Adı</th>
          <th>İzinler</th>
          <th>İzin Ekle</th>
          <th>İzin Kaldır</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="doctor in doctors" :key="doctor.id">
          <td>{{ doctor.user.firstName }} {{ doctor.user.lastName }}</td>
          <td>
            <button @click="showLeaves(doctor.id)">İzinleri Gör</button>
          </td>
          <td>
            <button @click="addLeave(doctor.id)">İzin Ekle</button>
          </td>
          <td>
            <button @click="removeLeave(doctor.id)">İzin Kaldır</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      doctors: []
    };
  },
  methods: {
    async fetchDoctors() {
      try {
        const response = await axios.get('http://localhost:5229/api/admin/doctors');
        this.doctors = response.data;
      } catch (error) {
        console.error(error);
      }
    },
    goToCreateDoctor() {
      this.$router.push('/doctor-create');
    },
    async addLeave(doctorId) {
      const date = prompt('İzin tarihi girin (yyyy-mm-dd):');
      try {
        const response = await axios.post('http://localhost:5229/api/admin/doctors/add-leave', {
          doctorId,
          leaveDate: date
        });
        alert('İzin Eklendi');
        this.fetchDoctors(); // Listeyi güncelle
      } catch (error) {
        alert('Hata: ' + error.response.data);
      }
    },
    async removeLeave(doctorId) {
      const date = prompt('Kaldırılacak izin tarihini girin (yyyy-mm-dd):');
      try {
        const response = await axios.delete('http://localhost:5229/api/admin/doctors/remove-leave', {
          data: {
            doctorId,
            leaveDate: date
          }
        });
        alert('İzin Kaldırıldı');
        this.fetchDoctors(); // Listeyi güncelle
      } catch (error) {
        alert('Hata: ' + error.response.data);
      }
    },
    async showLeaves(doctorId) {
      try {
        const response = await axios.get(`http://localhost:5229/api/admin/doctors/leaves/${doctorId}`);
        alert('İzinler: ' + JSON.stringify(response.data));
      } catch (error) {
        alert('Hata: ' + error.response.data);
      }
    }
  },
  mounted() {
    this.fetchDoctors();
  }
};
</script>
