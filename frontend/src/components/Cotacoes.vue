<template>
  <div class="box">
    <h3 class="box-title">Cotações em Tempo Real</h3>

    <div class="cotacao-item">
      <span class="moeda">USD/BRL</span>
      <span class="valor">R$ {{ dolarValor }}</span>
    </div>
    <div class="cotacao-item">
      <span class="variacao" :class="dolarVariacao >= 0 ? 'positiva' : 'negativa'">
        {{ dolarVariacao >= 0 ? '↑' : '↓' }} {{ Math.abs(dolarVariacao).toFixed(2) }}%
      </span>
      <span class="horario">{{ horario }}</span>
    </div>

    <div class="divisor"></div>

    <div class="cotacao-item">
      <span class="moeda">EUR/BRL</span>
      <span class="valor">R$ {{ euroValor }}</span>
    </div>
    <div class="cotacao-item">
      <span class="variacao" :class="euroVariacao >= 0 ? 'positiva' : 'negativa'">
        {{ euroVariacao >= 0 ? '↑' : '↓' }} {{ Math.abs(euroVariacao).toFixed(2) }}%
      </span>
      <span class="horario">{{ horario }}</span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Cotacoes',
  data() {
    return {
      dolarValor: 5.45,
      dolarVariacao: 0.15,
      euroValor: 6.20,
      euroVariacao: 0.22,
      horario: new Date().toLocaleTimeString()
    };
  },
  mounted() {
    setInterval(() => {
      this.dolarVariacao = Math.random() * 0.3 - 0.15;
      this.euroVariacao = Math.random() * 0.3 - 0.15;

      this.dolarValor = (5.45 * (1 + this.dolarVariacao / 100)).toFixed(2);
      this.euroValor = (6.20 * (1 + this.euroVariacao / 100)).toFixed(2);
      this.horario = new Date().toLocaleTimeString();
    }, 5000);
  }
};
</script>

