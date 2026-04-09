# Korp — teste técnico (fase 1: backend)

Monorepo com dois microsserviços .NET 8 (**Estoque** e **Faturamento**), PostgreSQL, integração HTTP na impressão da nota e testes unitários. O frontend Angular fica para uma fase posterior.

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (PostgreSQL + pgAdmin)

## Banco de dados e pgAdmin

Na raiz do repositório:

```bash
docker compose up -d
```

- **PostgreSQL**: `localhost:5432`, usuário `korp`, senha `korp_dev_change_me`
- Bancos: `db_estoque`, `db_faturamento` (criados pelo script em `docker/postgres-init/`)
- **pgAdmin**: http://localhost:5050 — e-mail `admin@korp.local`, senha `admin_dev_change_me`  
  Ao registrar o servidor, use host `host.docker.internal` (Windows/macOS) ou o IP da máquina, porta `5432`, usuário/senha acima.

## Executar as APIs

Em terminais separados, a partir da pasta `backend/`:

```bash
dotnet run --project Estoque/Estoque.Api/Estoque.Api.csproj --launch-profile http
```

- Swagger Estoque: http://localhost:5001/swagger

```bash
dotnet run --project Faturamento/Faturamento.Api/Faturamento.Api.csproj --launch-profile http
```

- Swagger Faturamento: http://localhost:5002/swagger

Em **Development**, as migrations do EF Core são aplicadas automaticamente na subida da API (veja `Program.cs` de cada serviço).

A URL da API de estoque consumida pelo Faturamento está em `Faturamento.Api/appsettings.json` → `Services:Estoque` (`http://localhost:5001`). Suba **sempre o Estoque antes** do Faturamento ao testar impressão.

## Fluxo sugerido (Swagger)

1. **Estoque**: `POST /api/Produtos` — cadastrar produtos (código, descrição, saldo inicial).
2. **Faturamento**: `POST /api/NotasFiscais` — criar nota (número sequencial gerado no banco).
3. **Faturamento**: `POST /api/NotasFiscais/{id}/itens` — informar `produtoId` (id do produto no Estoque) e `quantidade`.
4. **Faturamento**: `POST /api/NotasFiscais/{id}/imprimir` — deduz estoque via HTTP e fecha a nota.  
   - Se o Estoque estiver inacessível ou der timeout, a API retorna **503** com `ProblemDetails`.

## Testes

```bash
cd backend
dotnet test Korp.slnx
```

## Solução .NET

Arquivo da solution: `backend/Korp.slnx`.

## Evoluções opcionais (não implementadas aqui)

- **Concorrência**: a dedução em lote no Estoque usa transação com isolamento **Serializable**, o que reduz corridas em saldo; para cenários extremos, avaliar concorrência otimista (`xmin` / versão) ou política de retry explícita.
- **Idempotência na impressão**: pode ser adicionada com chave de idempotência (header ou registro) para evitar dedução dupla em reenvios.

## Próximos passos

- SPA **Angular** na pasta `frontend/` (CORS nas APIs na porta `4200` quando for o caso).
