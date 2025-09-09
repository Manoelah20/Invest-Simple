<template>
    <div class="box carteira-box">
      <h3 class="box-title">Minha Carteira</h3>
      <div v-for="(ativo, index) in carteira" :key="index" class="carteira-item">
        <div class="ativo">{{ ativo.nome }}</div>
        <div class="categoria">{{ ativo.categoria }}</div>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: 'Carteira',
    data() {
      return {
        carteira: [
          { nome: "TESOURO SELIC 2023", categoria: "Renda Fixa" },
          { nome: "ODB BANK 110% ODB", categoria: "Renda Fixa" },
          { nome: "LOI GREEN BANK", categoria: "Renda Fixa" },
          { nome: "PETRA – PETROBRÁS", categoria: "Renda Variável" },
          { nome: "ITUBA – ITAÚ", categoria: "Renda Variável" }
        ]
      }
    }
  }
  </script>
  
  <style scoped>
  .carteira-box {
    border-top-color: #2ecc71;
  }
  
  .carteira-item {
    padding: 10px 0;
    border-bottom: 1px solid #f0f0f0;
  }
  
  .carteira-item:last-child {
    border-bottom: none;
  }
  
  .ativo {
    font-weight: 500;
  }
  
  .categoria {
    font-size: 0.85rem;
    color: #7f8c8d;
    margin-top: 3px;
  }
  </style>