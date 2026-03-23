import axios from 'axios';
export const api = axios.create({ baseURL: 'https://api.exemplo.com' });

// Dados mock para quando o backend não estiver disponível
const mockCotacoes = [
  { 
    nome: 'Dólar Americano', 
    icone: '💵', 
    valor: 'R$ 5,45', 
    variacao: 0.15,
    simbolo: 'USD'
  },
  { 
    nome: 'Euro', 
    icone: '💶', 
    valor: 'R$ 6,20', 
    variacao: -0.08,
    simbolo: 'EUR'
  },
  { 
    nome: 'Bitcoin', 
    icone: '₿', 
    valor: 'R$ 352.450', 
    variacao: 2.35,
    simbolo: 'BTC'
  },
  { 
    nome: 'Ethereum', 
    icone: '🔷', 
    valor: 'R$ 18.920', 
    variacao: 1.85,
    simbolo: 'ETH'
  },
  { 
    nome: 'Ibovespa', 
    icone: '📊', 
    valor: '128.450 pts', 
    variacao: 0.65,
    simbolo: 'IBOV'
  },
  { 
    nome: 'Ouro', 
    icone: '🥇', 
    valor: 'US$ 1.985', 
    variacao: -0.25,
    simbolo: 'GOLD'
  }
];

const mockCarteira = [
  { 
    ativo: 'TESOURO SELIC 2026', 
    tipo: 'Renda Fixa',
    valor: 'R$ 5.000,00', 
    rentabilidade: '+0,85%',
    risco: 'Baixo',
    icone: '🏦',
    detalhes: 'Tesouro Direto pós-fixado'
  },
  { 
    ativo: 'CDB BANK 110% CDI', 
    tipo: 'Renda Fixa',
    valor: 'R$ 3.000,00', 
    rentabilidade: '+1,20%',
    risco: 'Baixo',
    icone: '💰',
    detalhes: 'CDB 2 anos'
  },
  { 
    ativo: 'LCI GREEN BANK', 
    tipo: 'Renda Fixa',
    valor: 'R$ 4.200,00', 
    rentabilidade: '+0,95%',
    risco: 'Baixo',
    icone: '🌱',
    detalhes: 'Isento de IR'
  },
  { 
    ativo: 'PETR4 - PETROBRAS', 
    tipo: 'Ações',
    valor: 'R$ 2.800,00', 
    rentabilidade: '+2,35%',
    risco: 'Médio',
    icone: '🛢️',
    detalhes: 'Setor Energético'
  },
  { 
    ativo: 'ITUB4 - ITAÚ', 
    tipo: 'Ações',
    valor: 'R$ 3.500,00', 
    rentabilidade: '+1,80%',
    risco: 'Médio',
    icone: '🏦',
    detalhes: 'Setor Bancário'
  }
];

export const useApi = () => {
  const useMock = ref(false);

  const checkBackend = async () => {
    try {
      // Timeout de 2 segundos para não travar o app
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 2000);
      
      const response = await fetch('http://localhost:3001/health', {
        signal: controller.signal
      });
      
      clearTimeout(timeoutId);
      return response.ok;
    } catch {
      useMock.value = true;
      return false;
    }
  };

  const getCotacoes = async () => {
    try {
      const isBackendUp = await checkBackend();
      
      if (!isBackendUp) {
        console.log('📡 Usando dados mock (backend offline)');
        return mockCotacoes;
      }

      const response = await fetch('http://localhost:3001/api/cotacoes');
      const data = await response.json();
      
      // Converte os dados da API para o formato do frontend
      return data.map(item => ({
        nome: item.nome || item.simbolo,
        icone: getIcone(item.simbolo),
        valor: formatarValor(item.valor, item.simbolo),
        variacao: item.variacao || 0,
        simbolo: item.simbolo
      }));
      
    } catch (error) {
      console.log('📡 Fallback para dados mock');
      return mockCotacoes;
    }
  };

  const getCarteira = async () => {
    try {
      const isBackendUp = await checkBackend();
      
      if (!isBackendUp) {
        console.log('📡 Usando carteira mock (backend offline)');
        return mockCarteira;
      }

      const response = await fetch('http://localhost:3001/api/carteira');
      const data = await response.json();
      return data;
      
    } catch (error) {
      console.log('📡 Fallback para carteira mock');
      return mockCarteira;
    }
  };

  // Funções auxiliares
  const getIcone = (simbolo) => {
    const icones = {
      'USD': '💵', 'EUR': '💶', 'BTC': '₿', 'ETH': '🔷',
      'IBOV': '📊', 'GOLD': '🥇'
    };
    return icones[simbolo] || '💰';
  };

  const formatarValor = (valor, simbolo) => {
    if (simbolo === 'USD' || simbolo === 'EUR') {
      return `R$ ${valor.toFixed(2).replace('.', ',')}`;
    }
    if (simbolo === 'BTC' || simbolo === 'ETH') {
      return `R$ ${valor.toLocaleString('pt-BR')}`;
    }
    if (simbolo === 'GOLD') {
      return `US$ ${valor.toFixed(2)}`;
    }
    return `${valor} pts`;
  };

  return {
    getCotacoes,
    getCarteira,
    useMock
  };
};