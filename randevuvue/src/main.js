import { createApp } from 'vue';
import App from './App.vue';
import router from './router'; // router.js'den import ediyoruz

const app = createApp(App);

// Vue Router'ı uygulamaya dahil et
app.use(router);

app.mount('#app');
