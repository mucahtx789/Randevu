<template>
  <div class="container">
    <h1>Randevularım</h1>

    <button @click="goToCreateAppointment">Yeni Randevu Al</button>

    <div v-if="appointments.length === 0">Henüz randevunuz yok.</div>

    <ul>
      <li v-for="appointment in appointments"
          :key="appointment.id"
          :class="getAppointmentClass(appointment)">
        <strong>{{ formatDateTime(appointment.appointmentTime) }}</strong><br>
        Uzmanlık: {{ appointment.specialization }}<br>
        Deneyim: {{ appointment.experienceLevel }}<br>
        Doktor: {{ appointment.doctorFullName }}<br>

        <!-- Şartlı buton gösterimi -->
        <button v-if="canCancel(appointment.appointmentTime)"
                @click="cancelAppointment(appointment.id)">
          İptal Et
        </button>

        <hr>
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
    getAppointmentClass(appointment) {
      const now = new Date();
      const apptTime = new Date(appointment.appointmentTime);
      const diffMs = apptTime - now;
      const diffHours = diffMs / (1000 * 60 * 60);

      if (apptTime < now) {
        return 'past'; // kırmızı
      } else if (diffHours < 1) {
        return 'soon'; // sarı
      } else {
        return 'future'; // yeşil
      }
    },
    canCancel(appointmentTime) {
      const now = new Date();
      const apptTime = new Date(appointmentTime);
      const diffMs = apptTime - now;
      const diffHours = diffMs / (1000 * 60 * 60);
      return apptTime > now && diffHours >= 1;
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
  li.past {
    background-color: #f8d7da; /* kırmızımsı */
    padding: 10px;
  }

  li.soon {
    background-color: #fff3cd; /* sarımsı */
    padding: 10px;
  }

  li.future {
    background-color: #d4edda; /* yeşilimsi */
    padding: 10px;
  }

</style>
