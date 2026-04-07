# Invest Simples - Deploy no Vercel

## Configurações realizadas para deploy no Vercel:

### 1. Estrutura de Arquivos
- `vercel.json` configurado para monorepo
- Backend adaptado para serverless functions
- Frontend configurado para build estático

### 2. Configurações aplicadas:

#### vercel.json
```json
{
  "version": 2,
  "builds": [
    {
      "src": "frontend/package.json",
      "use": "@vercel/static-build",
      "config": { "distDir": "dist" }
    },
    {
      "src": "backend/api/index.js",
      "use": "@vercel/node"
    }
  ],
  "routes": [
    { "src": "/api/(.*)", "dest": "/backend/api/index.js" },
    { "src": "/(.*)", "dest": "/frontend/$1" }
  ]
}
```

#### Backend Serverless
- Criado `backend/api/index.js` para Vercel functions
- Removido listener de porta do server original
- Mantidas todas as rotas da API

### 3. Próximos passos para deploy:

1. **Instalar Vercel CLI** (se ainda não tiver):
   ```bash
   npm i -g vercel
   ```

2. **Fazer login no Vercel**:
   ```bash
   vercel login
   ```

3. **Deploy do projeto**:
   ```bash
   vercel --prod
   ```

### 4. Variáveis de ambiente (se necessário):
- Configure no dashboard do Vercel em Settings > Environment Variables

### 5. Endpoints após deploy:
- Frontend: `https://seu-projeto.vercel.app`
- API: `https://seu-projeto.vercel.app/api/*`

A configuração está pronta para deploy no Vercel!
