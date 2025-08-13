
# ECommerce.Products — Guia de Execução (Backend + Frontend)

Passo a passo para rodar **backend (.NET 8 + Dapper + PostgreSQL + Docker)** e **frontend (Angular 16)** do CRUD de Produtos.

---

## 📦 Pré-requisitos

- **.NET SDK 8.0+**
- **Node.js 18+** e **npm**
- **Angular CLI 16**
  ```bash
  npm i -g @angular/cli@16
  ```
- **Docker** e **Docker Compose** (recomendado para o banco)
- **psql** (cliente PostgreSQL) caso vá aplicar os scripts via terminal

---

## 🗂️ Estrutura arquitetural (backend)

```
ECommerce.Products/
├─ src/
│  ├─ ECommerce.Products.API/
│  ├─ ECommerce.Products.Application/
│  ├─ ECommerce.Products.Domain/
│  └─ ECommerce.Products.Infrastructure/
├─ tests/
│  └─ ECommerce.Products.Tests/
├─ scripts/
│  ├─ 001_init_schema.sql
│  └─ 002_seed_departments.sql
└─ docker-compose.yml
```

---

## 🚀 Backend (.NET + PostgreSQL + Docker)

### 1) Subir o PostgreSQL (via Docker)
Na raiz do projeto:

```bash
docker compose up -d
```

Isso cria um Postgres com:
- **Database:** `ecommerce`
- **User/Password:** `ecommerce`
- **Porta:** `5432`

> Se usar instalação local do Postgres, ajuste a `ConnectionString` no `appsettings.json`.

### 2) Criar schema e seed
Aplicar os scripts (primeira vez):

```bash
psql -h localhost -U ecommerce -d ecommerce -f scripts/001_init_schema.sql
psql -h localhost -U ecommerce -d ecommerce -f scripts/002_seed_departments.sql
```

> Senha padrão: `ecommerce`

### 3) ConnectionString
Arquivo `src/ECommerce.Products.API/appsettings.json`:

```json
{
  "DbOptions": {
    "ConnectionString": "Host=localhost;Port=5432;Database=ecommerce;Username=ecommerce;Password=ecommerce"
  },
  "Swagger": {
    "Title": "ECommerce Products API",
    "Version": "v1"
  },
  "AllowedHosts": "*"
}
```

### 4) Rodar a API
No diretório `src/ECommerce.Products.API`:

```bash
dotnet restore
dotnet build
dotnet run --urls http://localhost:5000
```

Acessos:
- **Swagger:** `http://localhost:5000/swagger`
- **Endpoints principais:**
  - `GET  /api/departments`
  - `GET  /api/products`
  - `GET  /api/products/{id}`
  - `POST /api/products`
  - `PUT  /api/products/{id}`
  - `DELETE /api/products/{id}` (exclusão lógica)

### 5) Testes
Na raiz da solution:

```bash
dotnet test
```

---

## 🖥️ Frontend (Angular 16)

### 1) Instalar e subir o frontend
```bash
npm install
ng serve --open
```

Acesse: `http://localhost:4200`

---

## 🧪 Checklist de verificação rápida

1. **Backend** disponível em `http://localhost:5000/swagger`.
2. **GET /api/departments** retorna `200` no Swagger.
3. **Frontend** em `http://localhost:4200` carrega e preenche o dropdown de departamentos.

---

## ❓ Solução de problemas (FAQ)

**Erro Angular `HttpErrorResponse status: 0 Unknown Error` ao chamar `/api/departments`**  
- Geralmente é **CORS/preflight**. Confirme:
  - `app.UseCors("AllowLocal")` está **antes** do mapeamento de endpoints.
  - Origem `http://localhost:4200` está listada na política.
  - Se usa header customizado (`X-User`), mantenha `AllowAnyHeader()` e `AllowAnyMethod()`.
  - Alternativa: usar **proxy** do Angular.

**Portas diferentes**  
- Se a API não está em `5000`, ajuste `environment.apiBaseUrl` para a porta correta **ou** rode a API fixando a URL:
  ```bash
  dotnet run --urls http://localhost:5000
  ```

**Departamentos vazios**  
- Garanta que executou os scripts:
  ```bash
  psql -h localhost -U ecommerce -d ecommerce -f scripts/001_init_schema.sql
  psql -h localhost -U ecommerce -d ecommerce -f scripts/002_seed_departments.sql
  ```

**Falha ao conectar no Postgres**  
- Verifique se o container está rodando:
  ```bash
  docker ps
  ```
- Teste conexão:
  ```bash
  psql -h localhost -U ecommerce -d ecommerce -c "\dt"
  ```

---

