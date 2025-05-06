import { createApp } from 'vue';
import App from './App.vue';
import router from './router'; // router.js'den import ediyoruz
import vueRecaptcha from 'vue3-recaptcha2';

const app = createApp(App);

// Vue Router'ı uygulamaya dahil et
app.use(router);

// ReCaptcha bileşenini global olarak kaydediyoruz
app.component('vue-recaptcha', vueRecaptcha);

app.mount('#app');
