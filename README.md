Arquitetura do Projeto

A solução é dividida em quatro camadas, seguindo a regra de dependência onde as camadas externas apontam para as internas.

ApiJuros.Domain: A camada mais interna. Contém as entidades e a lógica de negócio mais pura, sem dependências externas. (Neste projeto simples, ela está vazia, mas serve como base para futuras regras de negócio complexas).

ApiJuros.Application: Contém a lógica da aplicação (casos de uso). Define interfaces que são implementadas pelas camadas externas e utiliza os DTOs (Data Transfer Objects) para trafegar dados.

ApiJuros.Infrastructure: Implementa as interfaces definidas na camada de Application. É responsável por detalhes técnicos como acesso a banco de dados, consumo de outras APIs, etc. (Neste projeto, a implementação do serviço está na camada de Application por simplicidade, mas em um caso real, ela estaria aqui).

ApiJuros.Presentation: A camada de entrada da aplicação. No nosso caso, é uma API Web ASP.NET Core que expõe os endpoints para o mundo externo e depende da camada de Application para executar as ações.


ApiJuros.Presentation -> ApiJuros.Infrastructure -> ApiJuros.Application -> ApiJuros.Domain


Tecnologias Utilizadas

.NET 8 (ou superior)

ASP.NET Core: Para a construção da API.

Swagger (Swashbuckle): Para documentação interativa da API.

FluentValidation: Para validação declarativa e robusta dos dados de entrada.


# Instale as dependências necessárias para validação e documentação.

# Adicione o FluentValidation ao projeto Application
dotnet add ApiJuros.Application/ApiJuros.Application.csproj package FluentValidation

# Adicione o Swagger e a integração do FluentValidation ao projeto Presentation
dotnet add ApiJuros.Presentation/ApiJuros.Presentation.csproj package Swashbuckle.AspNetCore
dotnet add ApiJuros.Presentation/ApiJuros.Presentation.csproj package FluentValidation.AspNetCore

Executar a Aplicação
Antes de executar pela primeira vez, é crucial confiar no certificado de desenvolvimento local para usar HTTPS.

#  Navegue para a pasta da API
cd ApiJuros.Presentation

#  Execute a aplicação
dotnet run

O terminal mostrará que o servidor está rodando, geralmente em https://localhost:7xxx e http://localhost:5xxx.


Como Usar a API
Após executar a aplicação, abra seu navegador e acesse a documentação interativa do Swagger.

URL do Swagger: https://localhost:7255/swagger (a porta pode variar).

Endpoint de Cálculo
POST /api/FinancialCalculator/calculate-compound-interest

Calcula o valor final e os juros totais de um investimento com base em um valor inicial, uma taxa de juros mensal e um período em meses.

Exemplo de Sucesso
Corpo da Requisição (Request Body):

{
  "initialValue": 1000,
  "monthlyInterestRate": 1.5,
  "timeInMonths": 24
}

Resposta de Sucesso (Código 200 OK):

{
  "investedAmount": 1000,
  "finalAmount": 1429.50,
  "totalInterest": 429.50,
  "timeInMonths": 24
}

Se você enviar dados inválidos (ex: valores negativos), a API retornará um erro 400 Bad Request com uma mensagem clara.

Corpo da Requisição (Inválido):

{
  "initialValue": -100,
  "monthlyInterestRate": 1,
  "timeInMonths": 12
}

Resposta de Erro (Código 400 Bad Request):

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "InitialValue": [
      "O valor inicial do investimento deve ser positivo."
    ]
  }
}