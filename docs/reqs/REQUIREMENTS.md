# 📋 REQUIREMENTS.md — Gestão Acervo

> **Projeto:** Gestão Acervo  
> **Backend:** Acervo.API (.NET 8 — Minimal API)  
> **Frontend:** acervo-web (Angular 17)  
> **Versão do Documento:** 1.0.0  
> **Data:** 2025-03-04

---

## 1. Requisitos Funcionais (RF)

### 1.1 Módulo: Autores

| ID    | Descrição |
|-------|-----------|
| RF-01 | O sistema deve permitir **cadastrar** um autor com os campos: `Nome` (obrigatório) e `Email` (obrigatório, único). |
| RF-02 | O sistema deve permitir **listar** todos os autores com suporte a paginação e filtro por nome. |
| RF-03 | O sistema deve permitir **buscar** um autor pelo seu identificador único (`Id`). |
| RF-04 | O sistema deve permitir **atualizar** os dados de um autor existente. |
| RF-05 | O sistema deve permitir **excluir** um autor, desde que ele não possua livros vinculados (soft delete ou rejeição com mensagem adequada). |

### 1.2 Módulo: Gêneros

| ID    | Descrição |
|-------|-----------|
| RF-06 | O sistema deve permitir **cadastrar** um gênero com o campo: `Nome` (obrigatório, único). |
| RF-07 | O sistema deve permitir **listar** todos os gêneros. |
| RF-08 | O sistema deve permitir **buscar** um gênero pelo seu `Id`. |
| RF-09 | O sistema deve permitir **atualizar** o nome de um gênero existente. |
| RF-10 | O sistema deve permitir **excluir** um gênero, desde que ele não possua livros vinculados (soft delete ou rejeição). |

### 1.3 Módulo: Livros

| ID    | Descrição |
|-------|-----------|
| RF-11 | O sistema deve permitir **cadastrar** um livro com os campos: `Título` (obrigatório), `ISBN` (obrigatório, único), `AnoPublicacao` (obrigatório), `AutorId` (FK, obrigatório) e `GeneroId` (FK, obrigatório). |
| RF-12 | O sistema deve permitir **listar** todos os livros com suporte a paginação e filtros por título, autor e gênero. |
| RF-13 | O sistema deve permitir **buscar** um livro pelo seu `Id`, retornando os dados do autor e gênero associados. |
| RF-14 | O sistema deve permitir **atualizar** os dados de um livro existente, incluindo a troca de autor e/ou gênero. |
| RF-15 | O sistema deve permitir **excluir** um livro pelo seu `Id`. |

---

## 2. Requisitos Não-Funcionais (RNF)

### 2.1 Banco de Dados

| ID     | Descrição |
|--------|-----------|
| RNF-01 | O banco de dados utilizado será o **SQL Server 2022**. |
| RNF-02 | O esquema do banco deve ser gerenciado integralmente via **EF Core Migrations**, sem scripts SQL manuais. |
| RNF-03 | Toda migration deve ser rastreável no controle de versão (Git). |

### 2.2 Backend

| ID     | Descrição |
|--------|-----------|
| RNF-04 | A API deve ser desenvolvida em **.NET 8 (C#)** utilizando o modelo **Minimal API**. |
| RNF-05 | A API deve implementar **versionamento de rotas** (`/api/v1/...`), com suporte a evolução para versões futuras sem quebra de contrato. |
| RNF-06 | A API deve ser documentada com **Swagger / OpenAPI 3.0**, com exemplos de request/response por endpoint. |
| RNF-07 | Todas as respostas devem seguir **HTTP Status Codes** semânticos (200, 201, 204, 400, 404, 409, 422, 500). |
| RNF-08 | A aplicação deve suportar múltiplos ambientes via **AppSettings** (`appsettings.Development.json` e `appsettings.Production.json`). |
| RNF-09 | Validações de entrada devem ser implementadas com **FluentValidation**. |
| RNF-10 | O mapeamento entre entidades e DTOs deve ser realizado com **Mapster**. |
| RNF-11 | O projeto deve possuir cobertura de **Testes de Unidade** com **xUnit**, **Moq** e **Shouldly**, cobrindo casos de sucesso e falha nos use cases/services. |

### 2.3 Frontend

| ID     | Descrição |
|--------|-----------|
| RNF-12 | O frontend deve ser uma **SPA (Single Page Application)** desenvolvida em **Angular 17**. |
| RNF-13 | O gerenciamento de estado global deve ser implementado com **NgRx** (Actions, Reducers, Effects, Selectors). |
| RNF-14 | A comunicação com a API deve ser centralizada em **Services** com tipagem via **Interfaces/Models** TypeScript. |
| RNF-15 | Um **HTTP Interceptor** deve ser implementado para tratamento centralizado de erros e (futuramente) injeção de tokens de autenticação. |
| RNF-16 | A navegação entre módulos deve ser gerenciada pelo **Angular Router** com lazy loading. |
| RNF-17 | O projeto deve suportar múltiplos ambientes via **`environment.ts`** e **`environment.prod.ts`**. |
| RNF-18 | O frontend deve possuir **Testes de Unidade** com **Jasmine/Karma** cobrindo componentes e serviços críticos. |

---

## 3. Regras de Negócio (RN)

### 3.1 Cardinalidade e Relacionamentos

| ID    | Descrição |
|-------|-----------|
| RN-01 | Um **Gênero** pode estar associado a **N Livros** (relação 1:N). |
| RN-02 | Um **Autor** pode estar associado a **N Livros** (relação 1:N). |
| RN-03 | Cada **Livro** pertence a **exatamente um Autor** e **exatamente um Gênero** (relação N:1 em ambos os casos). |

### 3.2 Restrições de Integridade e Validações

| ID    | Descrição |
|-------|-----------|
| RN-04 | O campo **`Email`** do Autor deve ser **único** no sistema. Tentativas de cadastro ou atualização com e-mail duplicado devem retornar `HTTP 409 Conflict`. |
| RN-05 | O campo **`ISBN`** do Livro deve ser **único** no sistema. Tentativas de cadastro ou atualização com ISBN duplicado devem retornar `HTTP 409 Conflict`. |
| RN-06 | O campo **`Nome`** do Gênero deve ser **único** no sistema. |
| RN-07 | É **proibida a exclusão física (hard delete)** de um **Autor** que possua livros vinculados. A tentativa deve retornar `HTTP 422 Unprocessable Entity` com mensagem descritiva. |
| RN-08 | É **proibida a exclusão física (hard delete)** de um **Gênero** que possua livros vinculados. A tentativa deve retornar `HTTP 422 Unprocessable Entity` com mensagem descritiva. |
| RN-09 | O **`AutorId`** e o **`GeneroId`** informados no cadastro/atualização de um Livro devem corresponder a registros **existentes e ativos** no banco de dados. Caso contrário, retornar `HTTP 404 Not Found`. |
| RN-10 | O campo **`AnoPublicacao`** do Livro deve ser um ano válido (maior que 1400 e não superior ao ano corrente). |
| RN-11 | Campos obrigatórios em branco ou nulos devem retornar `HTTP 400 Bad Request` com detalhamento dos campos inválidos. |

---

## 4. Respostas Padronizadas da API

Todas as respostas da API devem seguir o envelope padrão abaixo:

```json
{
  "success": true,
  "message": "Operação realizada com sucesso.",
  "data": { }
}
```

Em caso de erro:

```json
{
  "success": false,
  "message": "Descrição do erro.",
  "errors": ["Campo X é obrigatório.", "E-mail já cadastrado."]
}
```

---

## 5. Tabela de HTTP Status Codes Utilizados

| Status | Cenário |
|--------|---------|
| `200 OK` | Leitura e atualização bem-sucedidas |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Exclusão bem-sucedida |
| `400 Bad Request` | Dados de entrada inválidos (validação) |
| `404 Not Found` | Recurso não encontrado |
| `409 Conflict` | Violação de unicidade (ISBN, e-mail, nome) |
| `422 Unprocessable Entity` | Violação de regra de negócio (ex: exclusão com vínculo) |
| `500 Internal Server Error` | Erro inesperado no servidor |

---

*Documento gerado para o Desafio Técnico — Siemens | Projeto Gestão Acervo*
