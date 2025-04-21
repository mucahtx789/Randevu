<template>
  <div class="p-4 max-w-lg mx-auto">
    <h1 class="text-xl font-semibold mb-4">Randevu Oluştur</h1>

    <div class="mb-4 text-sm text-gray-700 bg-yellow-100 p-2 rounded">
      <p>🗓️ <strong>İzinli günler</strong> ve <strong>hafta sonları</strong> seçilemez.</p>
      <p>⏰ <strong>12:00 – 13:00 öğle arasında</strong> ve <strong>dolu saatler</strong> seçilemez.</p>
    </div>

    <!-- Branş seçimi -->
    <div class="mb-4">
      <label class="block mb-1 font-medium">Branş Seçin:</label>
      <select v-model="selectedBranch" @change="fetchDoctors" class="w-full border rounded p-2">
        <option disabled value="">-- Branş Seçin --</option>
        <option v-for="branch in branches" :key="branch" :value="branch">{{ branch }}</option>
      </select>
    </div>

    <!-- Doktor seçimi -->
    <div class="mb-4" v-if="doctors.length > 0">
      <label class="block mb-1 font-medium">Doktor Seçin:</label>
      <select v-model="selectedDoctorId" @change="fetchLeaveDays" class="w-full border rounded p-2">
        <option disabled value="">-- Doktor Seçin --</option>
        <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
          {{ doctor.fullName }} - {{ doctor.experienceLevel }}
        </option>
      </select>
    </div>

    <!-- Tarih seçimi -->
    <div class="mb-4" v-if="selectedDoctorId">
      <label class="block mb-1 font-medium">Tarih Seçin:</label>
      <input type="date"
             v-model="selectedDate"
             :min="minDate"
             class="w-full border rounded p-2"
             :class="{ 'border-red-500': !isDateSelectable }" />
    </div>

    <!-- Saatler -->
    <div class="grid grid-cols-3 gap-2">
      <button v-for="hour in availableHours"
              :key="hour.time"
              :disabled="hour.disabled"
              @click="selectTime(hour.time)"
              :class="[ 'p-2 rounded text-center transition duration-200',
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
            class="mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50">
      Randevuyu Kaydet
    </button>
    <!-- Dashboard git -->
    <button @click="goToDashboard"> Randevu Listesine Geri Dön</button>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        branches: [
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
        doctors: [],
        selectedBranch: '',
        selectedDoctorId: null,
        selectedDate: '',
        selectedTime: '',
        availableHours: [],
        leaveDays: [],
        isDateSelectable: true,
        minDate: new Date().toISOString().split('T')[0],
      };
    },
    methods: {
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
          console.error('Seçili branşta doktor yok:', error);
          alert('Seçili branşta doktor yok.');
        }
      },
      async fetchLeaveDays() {
        if (!this.selectedDoctorId) return;
        try {
          const res = await axios.get(`http://localhost:5229/api/admin/doctors/leaves/${this.selectedDoctorId}`);
          this.leaveDays = res.data.map(d => d.split('T')[0]);
          this.selectedDate = '';
          this.availableHours = [];
        } catch (error) {
          console.error('İzin günleri alınamadı:', error);
          alert('Doktor izin günleri yüklenemedi.');
        }
      },
      async fetchAvailableHours() {
        if (!this.selectedDate || !this.selectedDoctorId) return;

        const day = new Date(this.selectedDate).getDay();
        if (day === 0 || day === 6 || this.leaveDays.includes(this.selectedDate)) {
          this.availableHours = [];
          return;
        }

        try {
          const res = await axios.get(`http://localhost:5229/api/appointments/available-times?doctorId=${this.selectedDoctorId}&date=${this.selectedDate}`);
          this.availableHours = res.data.times.map(time => ({
            time,
            disabled: false
          }));
        } catch (error) {
          console.error('Saatler alınamadı:', error);
          alert('Uygun saatler yüklenemedi.');
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
          console.error('❌ Randevu oluşturma hatası:', error);
          if (error.response) {
            console.warn("📛 Doğrulama hataları:", error.response.data.errors);
            alert('Hata: ' + JSON.stringify(error.response.data.errors, null, 2));
          } else {
            alert('Beklenmeyen bir hata oluştu.');
          }
        }
      },
      goToDashboard() {
        this.$router.push('/dashboard');
      }
    },
    watch: {
      selectedDate(newDate) {
        if (!newDate) return;
        const day = new Date(newDate).getDay();
        const isWeekend = day === 0 || day === 6;
        const isLeaveDay = this.leaveDays.includes(newDate);

        this.isDateSelectable = !(isWeekend || isLeaveDay);

        if (!this.isDateSelectable) {
          alert("Seçtiğiniz gün hafta sonu ya da doktorun izinli olduğu bir gün.");
          this.selectedDate = '';
          this.availableHours = [];
        } else {
          this.fetchAvailableHours();
        }
      },
      
    }
  };
</script>

<style scoped>
  /* Saatler seçildiğinde görünüm */
  button.selected {
    background-color: #48BB78;
    color: white;
    font-weight: bold;
    transform: scale(1.1);
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
</style>
