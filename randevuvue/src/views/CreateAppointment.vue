<template>
  <div class="container">
    <h1>Randevu Oluştur</h1>

    <div class="info-box">
      <p>🗓️ <strong>İzinli günler</strong> ve <strong>hafta sonları</strong> seçilemez.</p>
      <p>⏰ <strong>12:00 – 13:00 öğle arasında</strong> ve <strong>dolu saatler</strong> seçilemez.</p>
    </div>

    <!-- Branş seçimi -->
    <div class="input-group">
      <label class="input-label">Branş Seçin:</label>
      <select v-model="selectedBranch" @change="fetchDoctors" class="input-field">
        <option disabled value="">-- Branş Seçin --</option>
        <option v-for="branch in branches" :key="branch" :value="branch">{{ branch }}</option>
      </select>
    </div>

    <!-- Doktor seçimi -->
    <div class="input-group" v-if="doctors.length > 0">
      <label class="input-label">Doktor Seçin:</label>
      <select v-model="selectedDoctorId" @change="fetchLeaveDays" class="input-field">
        <option disabled value="">-- Doktor Seçin --</option>
        <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
          {{ doctor.fullName }} - {{ doctor.experienceLevel }}
        </option>
      </select>
    </div>

    <!-- Tarih seçimi -->
    <div class="input-group" v-if="selectedDoctorId">
      <label class="input-label">Tarih Seçin:</label>
      <input type="date"
             v-model="selectedDate"
             :min="adjustedMinDate"
             class="input-field"
             :class="{ 'border-red-500': !isDateSelectable }" />
    </div>

    <!-- Saatler -->
    <div class="hour-selection">
      <button v-for="hour in availableHours"
              :key="hour.time"
              :disabled="hour.disabled"
              @click="selectTime(hour.time)"
              :class="[ 'time-button',
                        hour.disabled
                          ? 'bg-gray-300 cursor-not-allowed'
                          : selectedTime === hour.time
                            ? 'selected'
                            : 'bg-white border hover:bg-green-100'
              ]">
        {{ hour.time }}
      </button>
    </div>

    <!-- Kaydet -->
    <button @click="submitAppointment"
            :disabled="!selectedDoctorId || !selectedDate || !selectedTime"
            class="save-button">
      Randevuyu Kaydet
    </button>

    <!-- Dashboard git -->
    <button @click="goToDashboard" class="back-to-dashboard">Randevu Listesine Geri Dön</button>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        branches: [
          'Çocuk Sağlığı ve Hastalıkları', 'Dahiliye (İç Hastalıkları)', 'Kadın Hastalıkları ve Doğum',
          'Kardiyoloji', 'Kulak Burun Boğaz (KBB)', 'Ortopedi ve Travmatoloji', 'Göz Hastalıkları',
          'Nöroloji', 'Psikiyatri', 'Cildiye (Dermatoloji)', 'Göğüs Hastalıkları', 'Üroloji',
          'Genel Cerrahi', 'Beyin ve Sinir Cerrahisi', 'Fizik Tedavi ve Rehabilitasyon',
          'Enfeksiyon Hastalıkları', 'Anesteziyoloji ve Reanimasyon'
        ],
        doctors: [],
        selectedBranch: '',
        selectedDoctorId: null,
        selectedDate: '',
        selectedTime: '',
        availableHours: [],
        leaveDays: [],
        isDateSelectable: true
      };
    },
    computed: {
      adjustedMinDate() {
        const now = new Date();
        const hour = now.getHours();
        const minute = now.getMinutes();
        if (hour > 16 || (hour === 16 && minute >= 30)) {
          const tomorrow = new Date(now.getTime() + 24 * 60 * 60 * 1000);
          return tomorrow.toISOString().split('T')[0];
        }

        return now.toISOString().split('T')[0];
      }
    },
    methods: {
      // Tarihi sıfırlayarak (00:00) sadece gün kısmını döndüren fonksiyon
      normalizeDate(date) {
        const d = new Date(date);
        d.setHours(0, 0, 0, 0); // Saat kısmını sıfırlıyoruz
        return d.toISOString().split('T')[0]; // ISO formatında sadece tarih kısmını döndürüyoruz
      },

      async fetchDoctors() {
        if (!this.selectedBranch) return;
        try {
          const encodedBranch = encodeURIComponent(this.selectedBranch);
          const res = await axios.get(`http://localhost:5229/api/appointments/by-specialization/${encodedBranch}`);
          this.doctors = res.data.map(doctor => ({
            id: doctor.doctorId,
            fullName: doctor.fullName,
            experienceLevel: doctor.experienceLevel
          }));
          this.selectedDoctorId = null;
          this.selectedDate = '';
          this.availableHours = [];
          this.leaveDays = [];
        } catch (error) {
          alert('Seçili branşta doktor bulunamadı.');
        }
      },

      async fetchLeaveDays() {
        if (!this.selectedDoctorId) return;
        try {
          const res = await axios.get(`http://localhost:5229/api/admin/doctors/leaves/${this.selectedDoctorId}`);
          this.leaveDays = res.data.map(d => this.normalizeDate(d)); // Tarihleri normalize ediyoruz
          this.selectedDate = '';
          this.availableHours = [];
        } catch (error) {
          alert('Doktor izin günleri alınamadı.');
        }
      },

      async fetchAvailableHours() {
        if (!this.selectedDate || !this.selectedDoctorId) return;

        const normalizedDate = this.normalizeDate(this.selectedDate); // Tarih normalizasyonu
        if (this.leaveDays.includes(normalizedDate)) {
          this.availableHours = [];
          return;
        }

        try {
          const res = await axios.get(`http://localhost:5229/api/appointments/available-times?doctorId=${this.selectedDoctorId}&date=${normalizedDate}`);
          this.availableHours = res.data.times.map(time => ({
            time,
            disabled: false
          }));
        } catch (error) {
          alert('Uygun saatler alınamadı.');
        }
      },

      selectTime(time) {
        this.selectedTime = time;
      },

      async submitAppointment() {
        const patientId = localStorage.getItem('patientId');
        const appointment = {
          patientId: parseInt(patientId),
          doctorId: this.selectedDoctorId,
          appointmentTime: `${this.selectedDate}T${this.selectedTime}:00`
        };

        try {
          await axios.post('http://localhost:5229/api/appointments', appointment);
          alert('Randevu başarıyla oluşturuldu!');
          this.selectedBranch = '';
          this.doctors = [];
          this.selectedDoctorId = null;
          this.selectedDate = '';
          this.selectedTime = '';
          this.availableHours = [];
          this.$router.push('/dashboard');
        } catch (error) {
          alert('Randevu oluşturulamadı.');
        }
      },

      goToDashboard() {
        this.$router.push('/dashboard');
      }
    },
    watch: {
      selectedDate(newDate) {
        if (!newDate) return;
        const normalizedDate = this.normalizeDate(newDate); // Tarihi normalize et
        const isWeekend = new Date(normalizedDate).getDay() === 0 || new Date(normalizedDate).getDay() === 6;
        const isLeaveDay = this.leaveDays.includes(normalizedDate);

        this.isDateSelectable = !(isWeekend || isLeaveDay);

        if (!this.isDateSelectable) {
          alert("Seçtiğiniz gün hafta sonu ya da doktorun izinli olduğu bir gün.");
          this.selectedDate = '';
          this.availableHours = [];
        } else {
          this.fetchAvailableHours();
        }
      }
    }
  };
</script>

<style scoped>
  .container {
    max-width: 600px;
    margin: 100px auto;
    padding: 30px;
    background-color: rgba(255, 255, 255, 0.95);
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  h1 {
    margin-bottom: 30px;
    color: #1976d2;
    font-size: 24px;
    font-family: 'Arial', sans-serif;
  }

  .info-box {
    margin-bottom: 20px;
    background-color: #fff3cd;
    padding: 10px;
    border-radius: 5px;
  }

  .input-group {
    margin-bottom: 20px;
  }

  .input-label {
    font-weight: bold;
    margin-bottom: 5px;
    text-align: left;
    display: block;
  }

  .input-field {
    width: 100%;
    padding: 10px;
    border-radius: 5px;
    border: 1px solid #ccc;
    font-size: 16px;
  }

  .hour-selection {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 10px;
    margin-bottom: 20px;
  }

  .time-button {
    padding: 12px 20px;
    text-align: center;
    border: 1px solid #ccc;
    cursor: pointer;
    font-size: 16px;
    background-color: white; /* Normal buton rengi */
    color: black; /* Yazı rengi siyah */
    border-radius: 5px;
    font-family: 'Arial', sans-serif;
    transition: transform 0.2s ease, box-shadow 0.2s ease;
  }

    /* Seçili buton rengi */
    .time-button.selected {
      background-color: #48BB78; /* Yeşil arka plan */
      color: white;
      font-weight: bold;
      transform: scale(1.05);
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    /* Hover efektini  */
    .time-button:hover {
      background-color: yellow; /* Arka plan beyaz */
      color: black; /* Yazı rengi siyah */
      transform: none; /* Hover'da büyüme yok */
    }

    .time-button.bg-gray-300 {
      background-color: #f2f2f2;
    }

    /* Disabled butonlar */
    .time-button:disabled {
      background-color: #f2f2f2;
      cursor: not-allowed;
      color: #999;
    }

  .save-button {
    background-color: #2196f3;
    color: white;
    padding: 12px 20px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    margin-top: 20px;
    width: 100%;
    font-size: 16px;
    font-family: 'Arial', sans-serif;
  }

    .save-button:hover {
      background-color: #1976d2;
    }

  .back-to-dashboard {
    margin-top: 10px;
    font-size: 14px;
    color: #1976d2;
    padding: 12px 20px;
    border-radius: 5px;
    cursor: pointer;
    transition: color 0.3s, font-weight 0.3s; /* Hover efekti için geçiş */
  }

    .back-to-dashboard:hover {
      color: #1565c0; /* Hover durumunda daha koyu mavi renk */
      font-weight: bold; /* Hover durumunda yazıyı kalınlaştırdım */
    }
</style>


