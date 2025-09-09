<template>
  <div class="simulador">
    <h3>Simulador de Investimento</h3>

    <input
      v-model="valorInicial"
      type="number"
      class="input-simples"
      placeholder="Digite o valor para simular"
    />

    <button @click="simular" :disabled="loading" class="tema-btn">
      {{ loading ? 'Simulando...' : 'Simular Investimento' }}
    </button>

    <div v-if="resultado !== null">
      <p>Projeção em 12 meses: R$ {{ resultado }}</p>
      <p>Rentabilidade estimada: +{{ rentabilidade }}%</p>
    </div>
  </div>
</template>

<script>
import { calcularRetorno } from '../utils/calcularRetorno.js';

export default {
  name: 'Simulador',
  data() {
    return {
      valorInicial: 50,
      resultado: null,
      rentabilidade: null,
      loading: false
    };
  },
  methods: {
    esperar(ms) {
      return new Promise(resolve => setTimeout(resolve, ms));
    },
    async simular() {
      const valor = parseFloat(this.valorInicial);
      if (isNaN(valor) || valor <= 0) {
        alert('Digite um valor válido para simular.');
        return;
      }

      this.loading = true;
      try {
        await this.esperar(1000);
        const taxa = 0.1188;
        const retorno = calcularRetorno(valor, taxa, 12);
        this.resultado = retorno.toFixed(2);
        this.rentabilidade = (taxa * 100).toFixed(2);
      } catch (erro) {
        alert('Erro ao simular.');
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

