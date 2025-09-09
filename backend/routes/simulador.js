// backend/routes/simulador.js
const express = require('express');
const router = express.Router();

router.post('/', (req, res) => {
  const { valorInicial, anos, idade, tipoInvestimento, taxaAnual } = req.body;
  
  // Usa a taxa específica do tipo de investimento ou 10% como padrão
  const taxa = taxaAnual || 10;
  const taxaMensal = taxa / 12 / 100;
  const meses = anos * 12;
  const aplicacaoMensal = valorInicial;
  let saldo = 0;
  let totalInvestido = 0;

  // Simula aplicação mensal com juros compostos
  for (let i = 0; i < meses; i++) {
    saldo += aplicacaoMensal; // Aplica o valor mensal
    saldo += saldo * taxaMensal; // Aplica os juros
    totalInvestido += aplicacaoMensal;
  }

  res.json({ 
    retorno: saldo.toFixed(2),
    tipoInvestimento: tipoInvestimento || 'padrao',
    taxaAnual: taxa,
    idadeInicial: idade,
    idadeFinal: idade + anos,
    periodoAnos: anos,
    totalInvestido: totalInvestido.toFixed(2),
    aplicacaoMensal: aplicacaoMensal
  });
});

module.exports = router;
