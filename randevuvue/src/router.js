import { createRouter, createWebHistory } from 'vue-router';
import HomeView from './views/HomeView.vue';
import AdminLogin from './views/AdminLogin.vue';
import DoctorList from './views/DoctorList.vue';
import DoctorCreate from './views/DoctorCreate.vue';
import Dashboard from './views/Dashboard.vue';
import CreateAppointment from './views/CreateAppointment.vue';
import PatientRegister from './views/PatientRegister.vue';

const routes = [
  { path: '/', component: HomeView },
  { path: '/admin-login', component: AdminLogin },
  { path: '/doctor-list', component: DoctorList, meta: { requiresAuth: true, role: 'Admin' } },
  { path: '/doctor-create', component: DoctorCreate, meta: { requiresAuth: true, role: 'Admin' } },
  { path: '/dashboard', component: Dashboard, meta: { requiresAuth: true, role: 'Patient' } },
  { path: '/create-appointment', component: CreateAppointment, meta: { requiresAuth: true, role: 'Patient' } },
  { path: '/patient-register', component: PatientRegister }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// Route Guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!token) {
      next('/');
    } else if (to.meta.role && to.meta.role !== role) {
      next('/'); // yanlış rol ise anasayfaya gönder
    } else {
      next();
    }
  } else {
    next();
  }
});

export default router;
