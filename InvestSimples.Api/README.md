# InvestSimples API (.NET 8)

API Financeira em .NET 8 com autenticação JWT e Entity Framework Core, complementar ao projeto Vue.js + Node.js.

## 🚀 Tecnologias

- **.NET 8** / C#
- **Entity Framework Core** com SQL Server (LocalDB)
- **JWT Bearer Authentication**
- **BCrypt** para hash de senhas
- **Swagger/OpenAPI** para documentação

## 📁 Estrutura

```
InvestSimples.Api/
├── Controllers/
│   ├── AuthController.cs      # Login / Register / Me
│   ├── CotacoesController.cs  # GET /api/cotacoes
│   ├── CarteiraController.cs  # GET /api/carteira
│   └── SimuladorController.cs # POST /api/simulador
├── Models/
│   ├── Usuario.cs
│   ├── Ativo.cs
│   ├── Transacao.cs
│   └── Simulacao.cs
├── Data/
│   └── InvestContext.cs       # EF Core DbContext
├── Services/
│   └── JwtService.cs          # Geração/validação JWT
├── DTOs/
│   └── AuthDtos.cs
├── Program.cs                 # Configuração DI, Auth, Swagger
└── appsettings.json
```

## ⚙️ Como Rodar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (incluído no Visual Studio) ou Docker

### 1. Restaurar dependências
```bash
cd InvestSimples.Api
dotnet restore
```

### 2. Criar banco de dados
```bash
dotnet ef database update
```
*Ou apenas rode a aplicação - o `EnsureCreated()` cria automaticamente.*

### 3. Executar
```bash
dotnet run
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger

## 🔐 Autenticação

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"teste@investsimples.com","senha":"123456"}'
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "tipo": "Bearer",
  "expiraEm": 3600,
  "usuario": { "id": 1, "nome": "Usuário Teste", "email": "teste@investsimples.com" }
}
```

### Usar token nas requisições
```bash
curl -X GET http://localhost:5000/api/cotacoes \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

## 📡 Endpoints

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Login | ❌ |
| POST | `/api/auth/register` | Registro | ❌ |
| GET | `/api/auth/me` | Usuário logado | ✅ |
| GET | `/api/cotacoes` | Cotações (câmbio, cripto, índice) | ✅ |
| GET | `/api/cotacoes/todas` | Todas cotações | ✅ |
| GET | `/api/carteira` | Carteira do usuário | ✅ |
| GET | `/api/carteira/resumo` | Resumo da carteira | ✅ |
| POST | `/api/simulador` | Simular investimento | ✅ |
| POST | `/api/simulador/salvar` | Salvar simulação | ✅ |
| GET | `/api/simulador/historico` | Histórico simulações | ✅ |

## 🧪 Exemplo de Simulação

```bash
curl -X POST http://localhost:5000/api/simulador \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -d '{
    "valorInicial": 1000,
    "anos": 10,
    "idade": 30,
    "tipoInvestimento": "Ações",
    "taxaAnual": 12
  }'
```

**Resposta:**
```json
{
  "retorno": 232339.08,
  "tipoInvestimento": "Ações",
  "taxaAnual": 12,
  "idadeInicial": 30,
  "idadeFinal": 40,
  "periodoAnos": 10,
  "totalInvestido": 120000,
  "aplicacaoMensal": 1000
}
```

## 👤 Usuário de Teste (Seed)

| Email | Senha |
|-------|-------|
| teste@investsimples.com | 123456 |

## 🐳 Docker (Opcional)

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InvestSimples.Api.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InvestSimples.Api.dll"]
```

```bash
docker build -t investsimples-api .
docker run -p 5000:8080 investsimples-api
```

## 🔄 Integração com Frontend Vue.js

No frontend Vue.js, altere a baseURL da API:

```javascript
// src/services/api.js
const API_BASE = 'http://localhost:5000/api'  // .NET API
// const API_BASE = 'http://localhost:3000/api'  // Node.js API (original)
```

## 📝 Licença

MIT