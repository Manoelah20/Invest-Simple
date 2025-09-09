<template>
  <transition name="fade">
    <div v-if="carregando" class="loader">
      Carregando...
    </div>
    <div v-else id="app">
      <header>
        <h1>Invest Simple</h1>
        <button @click="alternarTema" class="tema-btn">
          Alternar Tema
        </button>

        <nav class="menu">
          <ul>
            <li><a href="#cotacoes">Cotações</a></li>
            <li><a href="#carteira">Carteira</a></li>
            <li><a href="#simulador">Simulador</a></li>
          </ul>
        </nav>
        <p>Seu aplicativo de investimento de maneira simples e rápida.</p>
      </header>

      <div class="box-container">
        <div class="box" id="cotacoes">
          <h2>Cotações</h2>
          <Cotacoes />
          <p class="fonte-observacao">
            Fonte: Dados simulados com base em médias de mercado.
          </p>
        </div>

        <div class="box" id="carteira">
          <h2>Carteira</h2>
          <Carteira />
        </div>

        <div class="box" id="simulador">
          <h2>Simulador</h2>
          <Simulador />
        </div>
      </div>

      <footer>
        <p>© 2025 Invest Simple — Todos os direitos reservados.</p>
      </footer>
    </div>
  </transition>
</template>

<script>
import Cotacoes from './components/Cotacoes.vue';
import Simulador from './components/Simulador.vue';
import Carteira from './components/Carteira.vue';
import { formatarMoeda, calcularRentabilidade } from './hooks';

export default {
  name: 'App',
  components: {
    Cotacoes,
    Simulador,
    Carteira
  },
  data() {
    return {
      carregando: true,
      temaClaro: true
    };
  },
  methods: {
    alternarTema() {
      this.temaClaro = !this.temaClaro;
      document.body.className = this.temaClaro ? 'tema-claro' : 'tema-escuro';
    }
  },
  mounted() {
    setTimeout(() => {
      this.carregando = false;
    }, 1000);
  }
};
</script>


