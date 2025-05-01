<template>
 
  <div class="page-container">
    <!-- Api limit Error Alert -->
    <div v-if="rateLimitError" class="error-message">
      ⚠️ Çok fazla istek gönderildi. Lütfen daha sonra tekrar deneyin.
    </div>
    <h1>Doktor Listesi</h1>
    <button @click="goToCreateDoctor">Doktor Kaydı Oluştur</button>
    <table>
      <thead>
        <tr>
          <th>Doktor</th>
          <th>İzinler</th>
          <th>İzin Ekle</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="doctor in doctors" :key="doctor.id">
          <td>{{ doctor.experienceLevel }} {{ doctor.fullName }} - {{ doctor.specialization }}</td>
          <td><button @click="showLeaves(doctor.id)">İzinleri Gör</button></td>
          <td><button @click="openLeaveModal(doctor.id)">İzin Ekle</button></td>
        </tr>
      </tbody>
    </table>
  </div>

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
        doctorLeaves: [],
        rateLimitError: false //api rate limit
      };
    },
    methods: {
      async fetchDoctors() {
        try {
          const response = await axios.get('http://localhost:5229/api/admin/doctors');
          this.doctors = response.data;
        } catch (error) {
          // Rate limit hatası kontrolü
          if (error.response && error.response.status === 429) {
            this.rateLimitError = true;
          } else {
            alert('Doktorlar alınırken hata: ' + error.message);
          }
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
          // Rate limit hatası kontrolü
          if (error.response && error.response.status === 429) {
            this.rateLimitError = true;
          } else {
            alert('Hata: ' + error.response.data);
          }
        }
      },
      async showLeaves(doctorId) {
        try {
          const response = await axios.get(`http://localhost:5229/api/admin/doctors/leaves/${doctorId}`);
          this.doctorLeaves = response.data;
          this.selectedDoctorId = doctorId;
          this.isLeavesModalOpen = true;
        } catch (error) {
          // Rate limit hatası kontrolü
          if (error.response && error.response.status === 429) {
            this.rateLimitError = true;
          } else {
            alert('İzinler alınamadı: ' + error.response.data);
          }
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
          // Rate limit hatası kontrolü
          if (error.response && error.response.status === 429) {
            this.rateLimitError = true;
          } else {
            alert('Kaldırma hatası: ' + error.response.data);
          }
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
  /* Sayfa genel stili */
  .page-container {
    max-width: 800px;
    margin: auto;
    padding: 2rem;
    font-family: "Segoe UI", sans-serif;
    background-color: rgba(255, 255, 255, 0.95);
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  }

  /* Başlık */
  h1 {
    text-align: center;
    color: #2c3e50;
    margin-bottom: 1.5rem;
  }

  /* Tablo stili */
  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
  }

  th, td {
    padding: 10px;
    text-align: left;
    border-bottom: 1px solid #ddd;
  }

  th {
    background-color: #f8f9fa;
    font-weight: bold;
    color: #2c3e50;
  }

  tr:hover {
    background-color: #f1f1f1;
  }

  /* Genel buton stili */
  button {
    padding: 8px 14px;
    margin: 6px;
    font-size: 14px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background 0.3s ease;
  }

    /* Ana buton (ör: Doktor Kaydı Oluştur) */
    button:first-of-type {
      background-color: #2e86de;
      color: white;
    }

      button:first-of-type:hover {
        background-color: #1e5fa9;
      }

  /* Diğer aksiyon butonları */
  td button {
    background-color: #6c757d;
    color: white;
  }

    td button:hover {
      background-color: #5a6268;
    }

  /* Modal genel */
  .modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.5);
    z-index: 9999;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .modal-content {
    background-color: white;
    padding: 2rem;
    border-radius: 8px;
    max-width: 400px;
    width: 90%;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  }

    .modal-content h2 {
      margin-bottom: 1rem;
      color: #2c3e50;
    }

    .modal-content ul {
      list-style: none;
      padding: 0;
    }

    .modal-content li {
      margin-bottom: 0.5rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    /* Modal butonları */
    .modal-content button {
      margin-top: 10px;
    }

      .modal-content button:first-of-type {
        background-color: #28a745; /* Ekle butonu: yeşil */
        color: black;
      }

        .modal-content button:first-of-type:hover {
          background-color: #218838;
        }

      .modal-content button:last-of-type {
        background-color: #dc3545; /* Kapat / İptal butonu: kırmızı */
        color: white;
      }

        .modal-content button:last-of-type:hover {
          background-color: #c82333;
        }

  /* Error message styling */
  .error-message {
    color: #e74c3c;
    background-color: #f8d7da;
    padding: 10px;
    border-radius: 5px;
    margin-bottom: 15px;
    font-weight: bold;
  }
</style>
