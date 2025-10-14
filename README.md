### 💹 API de Cálculo de Juros
Uma API RESTful robusta construída com .NET 8 para realizar cálculos de juros compostos, integrada com serviços externos para taxas de juros em tempo real, e totalmente containerizada com Docker.

✨ Features Principais
Autenticação e Autorização: Sistema seguro de registro e login de usuários com tokens JWT.

Cálculo de Juros: Endpoints para calcular juros compostos com taxas mensais ou anuais.

Taxa SELIC em Tempo Real: Integração com a API do Banco Central do Brasil para obter a taxa SELIC atual.

Performance Otimizada: Uso de cache distribuído com Redis para reduzir a latência em chamadas externas.

Monitoramento: Endpoints de Health Checks para verificar a saúde da API, do banco de dados e de serviços externos.

Containerização Completa: Ambiente de desenvolvimento e produção 100% containerizado com Docker e Docker Compose.

Logging Estruturado: Logs detalhados e configuráveis com Serilog.

Validação Robusta: Validação de dados de entrada utilizando FluentValidation.

🏛️ Arquitetura do Projeto
A solução segue os princípios da Clean Architecture, promovendo separação de conceitos, alta coesão e baixo acoplamento.

📦 ApiJuros.Domain: Camada mais interna. Contém as entidades de negócio (ApplicationUser, Simulation).

⚙️ ApiJuros.Application: Contém a lógica e os casos de uso da aplicação (Services), DTOs, interfaces e validações.

🏗️ ApiJuros.Infrastructure: Implementa as interfaces da camada de Application. Responsável pelos detalhes de infraestrutura, como acesso ao banco de dados (Entity Framework), geração de tokens e repositórios.

🌐 ApiJuros.Presentation: Camada de entrada da API (ASP.NET Core). Expõe os Controllers e endpoints para o mundo externo.


**Fluxo de Dependência:**
`Presentation` → `Application`  → `Domain` ←  `Infrastructure` 
                                       
### ✨ Tecnologias Utilizadas
  * **.NET 8** (ou superior)
  * **ASP.NET Core**: Para a construção da API.
  * **ASP.NET Core Identity**: Para gerenciamento de usuários e autenticação.
  * **JWT (JSON Web Tokens)**: Para autorização baseada em tokens.
  * **Entity Framework Core**: ORM para comunicação com o banco de dados.
  * **PostgreSQL**: Banco de dados relacional utilizado no projeto.
  * **Redis**: Para caching distribuído de dados, melhorando a performance.
  * **Docker & Docker Compose**: Para containerização completa da aplicação e seus serviços.
  * **Swagger (Swashbuckle)**: Para documentação interativa e teste dos endpoints da API.
  * **Serilog**: Para logging estruturado e configurável.
  * **FluentValidation**: Para validação robusta e declarativa dos dados de entrada.
  * **Health Checks**: Para monitoramento da saúde da aplicação e seus serviços.
  * **xUnit, Moq & FluentAssertions**: Ferramentas para a criação de testes unitários.

### 🚀 Como Executar

Opção 1: Usando Docker Compose (Recomendado)
Este é o método mais simples e garante um ambiente consistente.

Pré-requisitos:

Docker Desktop instalado e em execução.

Passos:

## 1\. Clone o repositório.

## 2\. Abra um terminal na pasta raiz do projeto (onde o arquivo docker-compose.yml está localizado).

## 3\. Execute o comando para construir as imagens e iniciar todos os containers (API, Banco de Dados e Redis):
```bash
docker-compose up --build -d
```
## 4\. Para parar todos os serviços, execute:
```bash
docker-compose down
```

Opção 2: Localmente sem Docker

## Pré-requisitos:

* **.NET 8 SDK**

* **Um servidor PostgreSQL rodando localmente.**

Passos:

## 1\. Clone o repositório.

## 2\. Atualize a `DefaultConnection` no arquivo ` appsettings.Development.json` com os dados do seu banco de dados local.

## 3\. Confie no certificado de desenvolvimento local (execute apenas uma vez):

```bash
dotnet dev-certs https --trust
```

### 4\. Navegue para a pasta do projeto principal e execute a aplicação:

```bash
cd ApiJuros.Presentation
dotnet run
```

### 💡 Como Usar a API
Após iniciar a aplicação, a documentação interativa estará disponível no Swagger.

* **URL do Swagger**: `http://localhost:8080/swagge1` (se estiver usando Docker)

🔐 Autenticação
A maioria dos endpoints requer um token de autenticação. O fluxo é o seguinte:

## 1\. `POST /api/v1/auth/register`: Registre um novo usuário.

## 2\. `POST /api/v1/auth/login`: Faça login com o usuário criado para obter um token JWT.

## 3\. Autorize no Swagger: Clique no botão "Authorize" no topo da página, e na janela que abrir, cole o token no formato `Bearer <seu_token_aqui>`.

## Endpoints Principais
`GET /api/v1/FinancialCalculator/current-interest-rate`
Retorna a taxa de juros SELIC atual, buscando da API do Banco Central. O resultado é mantido em cache no Redis para melhorar a performance.

* **Autenticação: Requerida**.

* **Resposta (200 OK)**:
```JASON
{
  "currentInterestRate": 10.5
}
```
`POST /api/v1/FinancialCalculator/calculate-with-selic-rate`

Calcula um investimento com base na taxa SELIC atual. O resultado da simulação é salvo no banco de dados.

* **Autenticação: Requerida**.
* **Corpo da Requisição**:
  ```JASON
  {
  "initialValue": 5000,
  "timeInMonths": 12
  }
  ```
    ```JASON
  {
  "investedAmount": 5000,
  "finalAmount": 5525.16,
  "totalInterest": 525.16,
  "timeInMonths": 12
  }
  ```

🩺 Monitoramento
A aplicação expõe um painel de Health Checks para monitorar a saúde dos seus componentes.

   **URL do Painel**: `http://localhost:8080/health-ui`

Este painel verifica continuamente se:

* A API do Banco Central está acessível.
* A conexão com o banco de dados PostgreSQL está funcional.
