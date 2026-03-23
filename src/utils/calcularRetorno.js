// src/utils/calcular-retorno.js
export function calcularRetorno(valor, taxa, meses) {
  return valor * Math.pow(1 + taxa, meses);
}

