// src/hooks.js

export function formatarMoeda(valor) {
    return `R$ ${valor.toFixed(2).replace('.', ',')}`;
  }
  
  export function calcularRentabilidade(inicial, final) {
    const rendimento = ((final - inicial) / inicial) * 100;
    return `${rendimento.toFixed(2)}%`;
  }
  