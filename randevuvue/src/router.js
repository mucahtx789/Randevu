import { createRouter, createWebHistory } from 'vue-router';
import HomeView from './views/HomeView.vue';
import AdminLogin from './views/AdminLogin.vue';
import DoctorList from './views/DoctorList.vue';
import DoctorCreate from './views/DoctorCreate.vue';
import Dashboard from './views/Dashboard.vue';

const routes = [
  { path: '/', component: HomeView },
  { path: '/admin-login', component: AdminLogin },
  { path: '/doctor-list', component: DoctorList, meta: { requiresAuth: true } },
  { path: '/doctor-create', component: DoctorCreate, meta: { requiresAuth: true } },
  { path: '/dashboard', component: Dashboard, meta: { requiresAuth: true } }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  if (to.matched.some(record => record.meta.requiresAuth) && !token) {
    next('/');
  } else {
    next();
  }
});

export default router;
