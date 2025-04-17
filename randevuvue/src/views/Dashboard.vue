<template>
  <div class="container">
    <h1>Randevularım</h1>

    <button @click="goToCreateAppointment">Yeni Randevu Al</button>

    <div v-if="appointments.length === 0">Henüz randevunuz yok.</div>

    <ul>
      <li v-for="appointment in appointments" :key="appointment.id">
        <strong>{{ formatDateTime(appointment.dateTime) }}</strong> -
        {{ appointment.doctorName }}
        <button @click="cancelAppointment(appointment.id)">İptal Et</button>
      </li>
    </ul>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      appointments: []
    };
  },
  methods: {
    async fetchAppointments() {
      const token = localStorage.getItem('token');
      try {
        const response = await axios.get('http://localhost:5229/api/appointments/mine', {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.appointments = response.data;
      } catch (error) {
        console.error(error);
        alert('Randevular alınamadı.');
      }
    },
    async cancelAppointment(appointmentId) {
      const token = localStorage.getItem('token');
      try {
        await axios.delete(`http://localhost:5229/api/appointments/${appointmentId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.fetchAppointments(); // Listeyi güncelle
      } catch (error) {
        console.error(error);
        alert('Randevu iptal edilemedi.');
      }
    },
    goToCreateAppointment() {
      this.$router.push('/create-appointment');
    },
    formatDateTime(dateTime) {
      const date = new Date(dateTime);
      return date.toLocaleString('tr-TR');
    }
  },
  mounted() {
    this.fetchAppointments();
  }
};
</script>

<style>
  .container {
    max-width: 600px;
    margin: auto;
  }

  button {
    margin-left: 10px;
  }
</style>
