import { createRouter, createWebHashHistory } from 'vue-router';
import Home from '../views/Home.vue';
import Simulador from '../components/Simulador.vue';

const routes = [
  { path: '/', component: Home },
  { path: '/simulador', component: Simulador }
];

export const router = createRouter({
  history: createWebHashHistory(),
  routes
});
