import Vue3Datepicker from 'vue3-datepicker';
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
        </tr>
      </thead>
      <tbody>
        <tr v-for="doctor in doctors" :key="doctor.id">
          <td>{{ doctor.user.fullName }}</td>
          <td><button @click="showLeaves(doctor.id)">İzinleri Gör</button></td>
          <td><button @click="openLeaveModal(doctor.id)">İzin Ekle</button></td>
        </tr>
      </tbody>
    </table>

    <!-- İzin Ekleme Modal -->
    <div v-if="isLeaveModalOpen" class="modal">
      <div class="modal-content">
        <h2>İzin Ekle</h2>
        <vue3-datepicker v-model="leaveDate"
                         :disabled-date="disablePastDates"
                         :format="'yyyy-MM-dd'" />
        <button @click="addLeave(leaveDate)">Ekle</button>
        <button @click="closeLeaveModal">İptal</button>
      </div>
    </div>

    <!-- İzinler Modal -->
    <div v-if="isLeavesModalOpen" class="modal">
      <div class="modal-content">
        <h2>Doktor İzinleri</h2>
        <ul>
          <li v-for="leave in doctorLeaves" :key="leave">
            {{ formatDate(leave) }}
            <button @click="removeLeave(leave)">Kaldır</button>
          </li>
        </ul>
        <button @click="closeLeavesModal">Kapat</button>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  import Vue3Datepicker from 'vue3-datepicker';

  export default {
    components: {
      Vue3Datepicker
    },
    data() {
      return {
        doctors: [],
        isLeaveModalOpen: false,
        isLeavesModalOpen: false,
        leaveDate: null,
        selectedDoctorId: null,
        doctorLeaves: []
      };
    },
    methods: {
      async fetchDoctors() {
        try {
          const response = await axios.get('http://localhost:5229/api/admin/doctors');
          this.doctors = response.data;
        } catch (error) {
          alert('Doktorlar alınırken hata: ' + error.message);
        }
      },
      goToCreateDoctor() {
        this.$router.push('/doctor-create');
      },
      openLeaveModal(doctorId) {
        this.selectedDoctorId = doctorId;
        this.leaveDate = this.getTomorrow();
        this.isLeaveModalOpen = true;
      },
      closeLeaveModal() {
        this.isLeaveModalOpen = false;
        this.leaveDate = null;
      },
      getTomorrow() {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        tomorrow.setHours(0, 0, 0, 0);
        return tomorrow;
      },
      disablePastDates(date) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return date <= today;
      },
      async addLeave(date) {
        if (!date) {
          alert("Tarih seçimi yapılmadı.");
          return;
        }
        try {
          await axios.post('http://localhost:5229/api/admin/doctors/add-leave', {
            doctorId: this.selectedDoctorId,
            leaveDate: date
          });
          alert("İzin eklendi");
          this.fetchDoctors();
          this.closeLeaveModal();
        } catch (error) {
          alert('Hata: ' + error.response.data);
        }
      },
      async showLeaves(doctorId) {
        try {
          const response = await axios.get(`http://localhost:5229/api/admin/doctors/leaves/${doctorId}`);
          this.doctorLeaves = response.data;
          this.selectedDoctorId = doctorId;
          this.isLeavesModalOpen = true;
        } catch (error) {
          alert('İzinler alınamadı: ' + error.response.data);
        }
      },
      async removeLeave(date) {
        try {
          await axios.delete('http://localhost:5229/api/admin/doctors/remove-leave', {
            data: {
              doctorId: this.selectedDoctorId,
              leaveDate: date
            }
          });
          alert("İzin kaldırıldı");
          this.showLeaves(this.selectedDoctorId);
        } catch (error) {
          alert('Kaldırma hatası: ' + error.response.data);
        }
      },
      closeLeavesModal() {
        this.isLeavesModalOpen = false;
        this.doctorLeaves = [];
      },
      formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric'
        });
      }
    },
    mounted() {
      this.fetchDoctors();
    }
  };
</script>

<style>
  .modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .modal-content {
    background: white;
    padding: 20px;
    border-radius: 8px;
  }
</style>
