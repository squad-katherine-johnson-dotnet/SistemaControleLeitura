# 📚 Sistema de Controle de Leitura

## 🎯 Visão Geral e Objetivo
 
API REST desenvolvida em .NET 10 para permitir que leitoras organizem seus livros, acompanhem o andamento das leituras e monitorem o progresso de suas metas anuais.

O sistema permite cadastrar uma conta, registrar livros, adicioná-los à estante, atualizar o status de leitura e acompanhar a quantidade de livros concluídos em cada ano.

Projeto desenvolvido pelo squad Katherine Johnson, como parte do bootcamp de Desenvolvimento .NET.

## 💻 Tecnologias Utilizadas
- **Backend:** C#, .NET 10 e ASP.NET Core Web API.
- **Persistência de dados:** Entity Framework Core 10 e SQLite.
- **Autenticação:** JWT (JSON Web Token).
- **Testes:** xUnit e Moq.
- **Documentação da API:** Swagger.

## 📁 Estrutura do Projeto

```text
SistemaControleLeitura/
├── Backend/
│   ├── SistemaControleLeitura.Api/
│   ├── SistemaControleLeitura.Application/
│   ├── SistemaControleLeitura.Domain/
│   └── SistemaControleLeitura.Infrastructure/
└── Tests/
    └── SistemaControleLeitura.Tests/
```

O backend está organizado em camadas, separando as responsabilidades da aplicação:

- **Api:** Controllers, configuração e inicialização da API.
- **Application:** Services e regras de negócio.
- **Domain:** Entidades e enums do domínio.
- **Infrastructure:** Persistência de dados, Entity Framework Core, Repositories e Migrations.
- **Tests:** Testes automatizados da aplicação.

## ⚙️ Como Executar o Backend

### Pré-requisitos

- .NET 10 SDK.
- EF Core CLI 10.0.14.

Caso a ferramenta EF Core CLI ainda não esteja instalada, execute:

```bash
dotnet tool install --global dotnet-ef --version 10.0.14
```

### Passo a passo

Na raiz do repositório, restaure e compile o projeto:

```bash
dotnet build
```

Configure as credenciais JWT utilizando User Secrets. Primeiro, acesse a pasta da API:

```bash
cd Backend/SistemaControleLeitura.Api
```

Gere uma chave secreta aleatória e configure as demais informações:

```powershell
$jwtSecret = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
dotnet user-secrets set "Jwt:SecretKey" $jwtSecret
dotnet user-secrets set "Jwt:Issuer" "SistemaControleLeitura"
dotnet user-secrets set "Jwt:Audience" "SistemaControleLeitura"
dotnet user-secrets set "Jwt:ExpirationInMinutes" "60"
```

Retorne à raiz do projeto:

```bash
cd ../..
```

Aplique as migrations para criar ou atualizar o banco de dados:

```bash
dotnet ef database update -p Backend/SistemaControleLeitura.Infrastructure -s Backend/SistemaControleLeitura.Api
```

Execute a API:

```bash
dotnet run --project Backend/SistemaControleLeitura.Api
```

### Swagger

Com a API em execução, acesse `/swagger` no endereço informado pelo terminal para consultar a documentação e testar os endpoints.

## ✨ Funcionalidades Implementadas

### Cadastro e autenticação de usuárias

- Cadastro de usuárias com validação dos dados.
- Validação de e-mail único.
- Armazenamento de senha com hash.
- Login com autenticação JWT.
- Proteção dos endpoints por autenticação.
- Isolamento dos dados por usuária autenticada.

### Gerenciamento de livros

- Cadastro de livros.
- Listagem dos livros cadastrados.
- Consulta de livro por ID.
- Atualização dos dados de um livro.
- Exclusão de livros, respeitando as regras de negócio.

### Gerenciamento da estante

- Adição de livros à estante.
- Listagem dos livros da estante.
- Filtro por status de leitura.
- Alteração do status de leitura.
- Validação das regras de transição de status.
- Início de releitura de livros concluídos.
- Remoção de livros da estante.

### Metas anuais

- Definição de meta anual de leitura.
- Acompanhamento da quantidade de livros concluídos.
- Cálculo do percentual de progresso.
- Cálculo da média mensal necessária para atingir a meta.

## 📌 Regras de Negócio

- O fluxo de leitura segue a ordem **Quero ler → Lendo → Lido**.
- Um livro marcado como **Lido** não pode voltar para **Lendo**.
- Para concluir uma leitura, é necessário informar a data de término, que não pode ser anterior à data de início.
- Um livro pode aparecer apenas uma vez na estante de cada usuária.
- Uma releitura cria um novo ciclo de leitura, preservando o histórico anterior.
- A leitura é contabilizada no ano em que foi concluída.
- Cada usuária pode possuir apenas uma meta por ano.
- Livros que estão na estante não podem ser excluídos.
- Cada usuária pode visualizar e gerenciar somente seus próprios dados.
- O e-mail deve ser único e a senha deve possuir no mínimo 8 caracteres.

## 📌 Endpoints da API

Os endpoints abaixo estão documentados no Swagger. As rotas protegidas exigem autenticação via JWT.

### Usuário

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/usuarios` | Cadastra uma usuária. |
| `POST` | `/api/autenticacao/login` | Realiza o login e retorna o token JWT. |

### Livros

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/livros` | Cadastra um livro. |
| `GET` | `/api/livros` | Lista os livros da usuária autenticada. |
| `GET` | `/api/livros/{id}` | Consulta um livro por ID. |
| `PUT` | `/api/livros/{id}` | Atualiza os dados de um livro. |
| `DELETE` | `/api/livros/{id}` | Exclui um livro. |

### Estante

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/estante` | Adiciona um livro à estante. |
| `GET` | `/api/estante` | Lista os livros da estante. |
| `PATCH` | `/api/leituras/{id}/status` | Altera o status de uma leitura. |
| `POST` | `/api/estante/{id}/leituras` | Inicia uma nova leitura (releitura). |
| `DELETE` | `/api/estante/{id}` | Remove um livro da estante. |

### Metas

| Método | Endpoint | Descrição |
|---|---|---|
| `PUT` | `/api/metas/{ano}` | Define ou atualiza a meta de leitura de um ano. |
| `GET` | `/api/metas/{ano}/progresso` | Consulta o progresso da meta anual. |

A documentação interativa, incluindo parâmetros e formatos de requisição e resposta, está disponível pelo Swagger durante a execução da API.


## 🧪 Testes Automatizados

O projeto possui testes automatizados utilizando **xUnit** e **Moq**, contemplando serviços e regras de negócio da aplicação.

Para executar os testes, utilize na raiz do repositório:

```bash
dotnet test
```

## 🧩 Modelo de Dados e Decisões de Modelagem

O domínio é composto pelas entidades principais:

- **Usuario:** representa a usuária cadastrada.
- **Livro:** armazena os dados dos livros cadastrados.
- **ItemEstante:** representa a presença de um livro na estante.
- **Leitura:** representa cada ciclo de leitura de um livro.
- **MetaAnual:** armazena a meta definida para determinado ano.

A separação entre `ItemEstante` e `Leitura` permite que um livro permaneça uma única vez na estante e tenha vários ciclos de leitura ao longo do tempo.

O status é armazenado na entidade `Leitura`, evitando duplicidade de informações. O status atual é determinado pela leitura mais recente.

A avaliação com nota e comentário, busca por autora e catálogo compartilhado foram definidos como funcionalidades extras, fora do MVP.

## 👩‍💻 Integrantes e Contribuições

- **Bianca Fernandes:** Cadastro, autenticação JWT e integração da base da API.
- **Mariana Brombilla:** Configuração do projeto e do banco de dados e gerenciamento de livros.
- **Yasmin Bezerra:** Gerenciamento da estante e status de leitura.
- **Rosana Lima:** Metas anuais e acompanhamento do progresso.

 
