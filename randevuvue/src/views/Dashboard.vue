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

<style scoped>
  /* Container, butonlar ve listeyi aynı stile getirdik */
  .container {
    max-width: 600px;
    margin: 100px auto;
    padding: 30px;
    background-color: rgba(255, 255, 255, 0.95); /* Yarı saydam beyaz */
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  /* Başlık */
  h1 {
    margin-bottom: 30px;
    color: #1976d2; /* Mavi tema rengi */
    font-size: 24px;
  }

  /* Butonlar */
  button {
    background-color: #2196f3;
    color: white;
    border: none;
    padding: 10px 18px;
    font-size: 16px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    margin: 5px;
    width: 100%;
  }

    button:hover {
      background-color: #1976d2;
    }

  /* Randevuların Durumuna Göre Arkaplan Renkleri */
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
