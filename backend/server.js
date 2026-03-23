const express = require('express');
const cors = require('cors');
const simuladorRoute = require('./routes/simulador');

const app = express();
const PORT = process.env.PORT || 3001;

// Configuração CORS para desenvolvimento e produção
const allowedOrigins = [
  'http://localhost:5173',
  'http://localhost:3000', 
  'http://127.0.0.1:5173',
  process.env.NODE_ENV === 'production' ? /^https:\/\/.*\.vercel\.app$/ : null
].filter(Boolean);

app.use(cors({
  origin: allowedOrigins,
  credentials: true
}));

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Rotas da API
app.use('/api/simulador', simuladorRoute);

// Rota para cotações em tempo real (mock)
app.get('/api/cotacoes', (req, res) => {
  const cotacoes = [
    { nome: 'USD/BRL', valor: (5.2 + Math.random() * 0.4).toFixed(2), variacao: (Math.random() * 4 - 2).toFixed(2) },
    { nome: 'EUR/BRL', valor: (5.6 + Math.random() * 0.4).toFixed(2), variacao: (Math.random() * 4 - 2).toFixed(2) },
    { nome: 'BTC/BRL', valor: (250000 + Math.random() * 10000).toFixed(2), variacao: (Math.random() * 8 - 4).toFixed(2) },
    { nome: 'IBOVESPA', valor: (120000 + Math.random() * 5000).toFixed(2), variacao: (Math.random() * 6 - 3).toFixed(2) },
    { nome: 'CDI', valor: (13.25 + Math.random() * 0.5).toFixed(2), variacao: (Math.random() * 2 - 1).toFixed(2) },
    { nome: 'SELIC', valor: (10.5 + Math.random() * 0.3).toFixed(2), variacao: (Math.random() * 1 - 0.5).toFixed(2) }
  ];
  
  res.json(cotacoes);
});

// Rota para carteira de investimentos (mock)
app.get('/api/carteira', (req, res) => {
  const carteira = [
    { ativo: 'PETR4', tipo: 'Ações', valorInvestido: 15000, rentabilidade: 8.5, risco: 'Alto' },
    { ativo: 'VALE3', tipo: 'Ações', valorInvestido: 12000, rentabilidade: 12.3, risco: 'Alto' },
    { ativo: 'CDB Banco X', tipo: 'Renda Fixa', valorInvestido: 25000, rentabilidade: 13.2, risco: 'Baixo' },
    { ativo: 'LCI Banco Y', tipo: 'Renda Fixa', valorInvestido: 18000, rentabilidade: 11.8, risco: 'Baixo' },
    { ativo: 'FII HGLG11', tipo: 'Fundos', valorInvestido: 8000, rentabilidade: 9.7, risco: 'Médio' }
  ];
  
  res.json(carteira);
});

// Rota principal
app.get('/', (req, res) => {
  res.json({ 
    message: 'API InvestSimples rodando 🚀',
    version: '1.0.0',
    endpoints: {
      simulador: '/api/simulador',
      cotacoes: '/api/cotacoes',
      carteira: '/api/carteira'
    }
  });
});

// Middleware de tratamento de erros
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).json({ error: 'Algo deu errado!' });
});

// Middleware para rotas não encontradas
app.use('*', (req, res) => {
  res.status(404).json({ error: 'Rota não encontrada' });
});

app.listen(PORT, () => {
  console.log(`🚀 Servidor rodando na porta ${PORT}`);
  console.log(`📊 API disponível em: http://localhost:${PORT}`);
  console.log(`🔗 Frontend deve rodar em: http://localhost:5173`);
});

