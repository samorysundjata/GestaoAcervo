# 📂 PROJECT_STRUCTURE.md — Gestão Acervo

> **Projeto:** Gestão Acervo  
> **Backend:** Acervo.API (.NET 8 — Minimal API)  
> **Frontend:** acervo-web (Angular 17)  
> **Versão do Documento:** 1.0.0  
> **Data:** 2025-03-04

---

## 1. Visão Geral do Repositório

```
gestao-acervo/
├── backend/
│   └── Acervo/                      → Solution .NET 8
├── frontend/
│   └── acervo-web/           → SPA Angular 17
├── .gitignore
└── README.md
```

---

## 2. Backend — Acervo.API

Solução .NET 8 organizada em quatro projetos seguindo Clean Architecture.

```
backend/
└── Acervo/
    ├── Acervo.sln
    │
    ├── Acervo.Domain/                          → Camada de Domínio (sem dependências externas)
    │   ├── Acervo.Domain.csproj
    │   ├── Entities/
    │   │   ├── Autor.cs                        → Entidade Autor
    │   │   ├── Genero.cs                       → Entidade Gênero
    │   │   └── Livro.cs                        → Entidade Livro
    │   ├── Interfaces/
    │   │   ├── Repositories/
    │   │   │   ├── IAutorRepository.cs
    │   │   │   ├── IGeneroRepository.cs
    │   │   │   └── ILivroRepository.cs
    │   │   └── Services/
    │   │       ├── IAutorService.cs
    │   │       ├── IGeneroService.cs
    │   │       └── ILivroService.cs
    │   └── Common/
    │       └── Result.cs                       → Padrão Result<T> para retorno de erros sem exceções
    │
    ├── Acervo.Infrastructure/                  → Camada de Infraestrutura (EF Core, SQL Server)
    │   ├── Acervo.Infrastructure.csproj
    │   ├── Data/
    │   │   ├── AcervoDbContext.cs              → DbContext principal
    │   │   └── Configurations/
    │   │       ├── AutorConfiguration.cs       → Fluent API para Autor
    │   │       ├── GeneroConfiguration.cs      → Fluent API para Gênero
    │   │       └── LivroConfiguration.cs       → Fluent API para Livro (índices, FK, Restrict)
    │   ├── Migrations/
    │   │   ├── 20250304000001_InitialCreate.cs
    │   │   └── AcervoDbContextModelSnapshot.cs
    │   ├── Repositories/
    │   │   ├── AutorRepository.cs
    │   │   ├── GeneroRepository.cs
    │   │   └── LivroRepository.cs
    │   └── DependencyInjection.cs              → Extension method para registro de serviços
    │
    ├── Acervo.Application/                     → Camada de Aplicação (Services, DTOs, ViewModels, Mappings)
    │   ├── Acervo.Application.csproj
    │   ├── Services/
    │   │   ├── AutorService.cs
    │   │   ├── GeneroService.cs
    │   │   └── LivroService.cs
    │   ├── DTOs/
    │   │   ├── Autor/
    │   │   │   ├── CreateAutorDto.cs
    │   │   │   └── UpdateAutorDto.cs
    │   │   ├── Genero/
    │   │   │   ├── CreateGeneroDto.cs
    │   │   │   └── UpdateGeneroDto.cs
    │   │   └── Livro/
    │   │       ├── CreateLivroDto.cs
    │   │       └── UpdateLivroDto.cs
    │   ├── ViewModels/
    │   │   ├── AutorViewModel.cs
    │   │   ├── GeneroViewModel.cs
    │   │   ├── LivroViewModel.cs
    │   │   └── LivroDetalheViewModel.cs        → Livro com Autor e Gênero expandidos
    │   ├── Mappings/
    │   │   ├── AutorMappingConfig.cs           → Mapster IRegister Config
    │   │   ├── GeneroMappingConfig.cs
    │   │   └── LivroMappingConfig.cs
    │   ├── Validators/
    │   │   ├── CreateAutorValidator.cs         → FluentValidation
    │   │   ├── UpdateAutorValidator.cs
    │   │   ├── CreateGeneroValidator.cs
    │   │   ├── UpdateGeneroValidator.cs
    │   │   ├── CreateLivroValidator.cs
    │   │   └── UpdateLivroValidator.cs
    │   └── DependencyInjection.cs
    │
    ├── Acervo.API/                             → Camada de Apresentação (Minimal API, Config)
    │   ├── Acervo.API.csproj
    │   ├── Program.cs                          → Entry point — pipeline e registro de serviços
    │   ├── Endpoints/
    │   │   ├── AutorEndpoints.cs               → MapGroup /api/v1/autores
    │   │   ├── GeneroEndpoints.cs              → MapGroup /api/v1/generos
    │   │   └── LivroEndpoints.cs               → MapGroup /api/v1/livros
    │   ├── Common/
    │   │   └── ApiResponse.cs                  → Envelope padronizado { success, message, data, errors }
    │   ├── appsettings.json
    │   ├── appsettings.Development.json        → ConnectionString local, Swagger habilitado
    │   └── appsettings.Production.json         → ConnectionString produção, logs configurados
    │
    └── Acervo.Tests/                           → Projeto de Testes de Unidade
        ├── Acervo.Tests.csproj
        ├── Services/
        │   ├── AutorServiceTests.cs
        │   ├── GeneroServiceTests.cs
        │   └── LivroServiceTests.cs
        └── Validators/
            ├── CreateAutorValidatorTests.cs
            ├── CreateGeneroValidatorTests.cs
            └── CreateLivroValidatorTests.cs
```

---

## 3. Entidades do Domínio

### 3.1 Autor

```csharp
// Acervo.Domain/Entities/Autor.cs
public class Autor
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }           // único — RN-04
    public ICollection<Livro> Livros { get; private set; } = new List<Livro>();
}
```

### 3.2 Gênero

```csharp
// Acervo.Domain/Entities/Genero.cs
public class Genero
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }            // único — RN-06
    public ICollection<Livro> Livros { get; private set; } = new List<Livro>();
}
```

### 3.3 Livro

```csharp
// Acervo.Domain/Entities/Livro.cs
public class Livro
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string ISBN { get; private set; }            // único — RN-05
    public int AnoPublicacao { get; private set; }
    public Guid AutorId { get; private set; }
    public Autor Autor { get; private set; }            // N:1
    public Guid GeneroId { get; private set; }
    public Genero Genero { get; private set; }          // N:1
}
```

---

## 4. Frontend — acervo-web

SPA Angular 17 organizada por features com NgRx para gerenciamento de estado.

```
frontend/
└── acervo-web/
    ├── angular.json
    ├── package.json
    ├── tsconfig.json
    │
    └── src/
        ├── main.ts                             → Bootstrap da aplicação
        ├── index.html
        ├── styles.scss                         → Estilos globais
        │
        ├── environments/
        │   ├── environment.ts                  → { production: false, apiUrl: 'http://localhost:5000/api' }
        │   └── environment.prod.ts             → { production: true, apiUrl: 'https://...' }
        │
        └── app/
            ├── app.config.ts                   → provideRouter, provideStore, provideHttpClient, interceptors
            ├── app.routes.ts                   → Rotas raiz com lazy loading por feature
            │
            ├── core/                           → Singleton: interceptors, guards, layout
            │   ├── interceptors/
            │   │   └── error.interceptor.ts    → Tratamento centralizado de erros HTTP
            │   └── layout/
            │       ├── navbar/
            │       │   ├── navbar.component.ts
            │       │   └── navbar.component.html
            │       └── footer/
            │           └── footer.component.ts
            │
            ├── shared/                         → Componentes, pipes e models reutilizáveis
            │   ├── components/
            │   │   ├── confirm-dialog/
            │   │   │   └── confirm-dialog.component.ts
            │   │   └── loading-spinner/
            │   │       └── loading-spinner.component.ts
            │   └── models/
            │       └── api-response.model.ts   → Interface ApiResponse<T>
            │
            └── features/
                │
                ├── autores/                    → Feature: Autores
                │   ├── autores.routes.ts        → Lazy routes: list / form / detail
                │   ├── models/
                │   │   ├── autor.model.ts       → Interface AutorViewModel
                │   │   ├── create-autor.dto.ts
                │   │   └── update-autor.dto.ts
                │   ├── services/
                │   │   └── autor.service.ts     → CRUD via HttpClient
                │   ├── store/
                │   │   ├── autores.actions.ts   → loadAutores, createAutor, deleteAutor, ...
                │   │   ├── autores.reducer.ts   → Estado: { autores[], loading, error }
                │   │   ├── autores.effects.ts   → Efeitos HTTP assíncronos
                │   │   └── autores.selectors.ts → selectAllAutores, selectAutorById, selectLoading
                │   └── components/
                │       ├── autores-list/
                │       │   ├── autores-list.component.ts    → Smart
                │       │   └── autores-list.component.html
                │       ├── autor-form/
                │       │   ├── autor-form.component.ts      → Smart (create/edit)
                │       │   └── autor-form.component.html
                │       └── autor-card/
                │           ├── autor-card.component.ts      → Dumb
                │           └── autor-card.component.html
                │
                ├── generos/                    → Feature: Gêneros (estrutura espelhada)
                │   ├── generos.routes.ts
                │   ├── models/
                │   │   ├── genero.model.ts
                │   │   ├── create-genero.dto.ts
                │   │   └── update-genero.dto.ts
                │   ├── services/
                │   │   └── genero.service.ts
                │   ├── store/
                │   │   ├── generos.actions.ts
                │   │   ├── generos.reducer.ts
                │   │   ├── generos.effects.ts
                │   │   └── generos.selectors.ts
                │   └── components/
                │       ├── generos-list/
                │       ├── genero-form/
                │       └── genero-card/
                │
                └── livros/                     → Feature: Livros (estrutura espelhada + selects de Autor/Gênero)
                    ├── livros.routes.ts
                    ├── models/
                    │   ├── livro.model.ts           → Interface LivroViewModel (com autorNome, generoNome)
                    │   ├── livro-detalhe.model.ts   → Interface LivroDetalheViewModel
                    │   ├── create-livro.dto.ts
                    │   └── update-livro.dto.ts
                    ├── services/
                    │   └── livro.service.ts
                    ├── store/
                    │   ├── livros.actions.ts
                    │   ├── livros.reducer.ts
                    │   ├── livros.effects.ts
                    │   └── livros.selectors.ts
                    └── components/
                        ├── livros-list/
                        │   ├── livros-list.component.ts     → Smart
                        │   └── livros-list.component.html
                        ├── livro-form/
                        │   ├── livro-form.component.ts      → Smart (popula selects de Autor e Gênero)
                        │   └── livro-form.component.html
                        └── livro-card/
                            ├── livro-card.component.ts      → Dumb
                            └── livro-card.component.html
```

---

## 5. Roteamento Angular

```typescript
// app/app.routes.ts
export const routes: Routes = [
  { path: '', redirectTo: 'livros', pathMatch: 'full' },
  {
    path: 'autores',
    loadChildren: () =>
      import('./features/autores/autores.routes').then(m => m.AUTORES_ROUTES)
  },
  {
    path: 'generos',
    loadChildren: () =>
      import('./features/generos/generos.routes').then(m => m.GENEROS_ROUTES)
  },
  {
    path: 'livros',
    loadChildren: () =>
      import('./features/livros/livros.routes').then(m => m.LIVROS_ROUTES)
  },
  { path: '**', redirectTo: 'livros' }
];
```

```typescript
// features/livros/livros.routes.ts
export const LIVROS_ROUTES: Routes = [
  { path: '', component: LivrosListComponent },
  { path: 'novo', component: LivroFormComponent },
  { path: ':id/editar', component: LivroFormComponent }
];
```

---

## 6. Endpoints da API — Referência Rápida

### Autores — `/api/v1/autores`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/v1/autores` | Lista todos os autores |
| `GET` | `/api/v1/autores/{id}` | Busca autor por Id |
| `POST` | `/api/v1/autores` | Cria novo autor |
| `PUT` | `/api/v1/autores/{id}` | Atualiza autor |
| `DELETE` | `/api/v1/autores/{id}` | Remove autor |

### Gêneros — `/api/v1/generos`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/v1/generos` | Lista todos os gêneros |
| `GET` | `/api/v1/generos/{id}` | Busca gênero por Id |
| `POST` | `/api/v1/generos` | Cria novo gênero |
| `PUT` | `/api/v1/generos/{id}` | Atualiza gênero |
| `DELETE` | `/api/v1/generos/{id}` | Remove gênero |

### Livros — `/api/v1/livros`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/v1/livros` | Lista todos os livros (com paginação) |
| `GET` | `/api/v1/livros/{id}` | Busca livro por Id (com Autor e Gênero) |
| `POST` | `/api/v1/livros` | Cria novo livro |
| `PUT` | `/api/v1/livros/{id}` | Atualiza livro |
| `DELETE` | `/api/v1/livros/{id}` | Remove livro |

---

## 7. Dependências Principais

### Backend (NuGet)

| Pacote | Versão | Finalidade |
|--------|--------|-----------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.x | ORM + SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 8.x | Migrations CLI |
| `FluentValidation.AspNetCore` | 11.x | Validação de DTOs |
| `Mapster` | 7.x | Mapeamento entidade ↔ DTO (alta performance, sem reflection) |
| `Swashbuckle.AspNetCore` | 6.x | Swagger / OpenAPI |
| `xUnit` | 2.x | Framework de testes |
| `Moq` | 4.x | Mocking para testes |
| `Shouldly` | 4.x | Asserções legíveis nos testes |

### Frontend (npm)

| Pacote | Versão | Finalidade |
|--------|--------|-----------|
| `@angular/core` | 17.x | Framework SPA |
| `@ngrx/store` | 17.x | Gerenciamento de estado |
| `@ngrx/effects` | 17.x | Side effects assíncronos |
| `@ngrx/entity` | 17.x | Normalização de coleções |
| `@ngrx/devtools` | 17.x | Redux DevTools |
| `@angular/material` | 17.x | Componentes UI |
| `jasmine-core` | 5.x | Testes de unidade |

---

*Documento gerado para o Desafio Técnico — Siemens | Projeto Gestão Acervo*
