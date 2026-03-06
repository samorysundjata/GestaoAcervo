# 🏛️ ARCHITECTURE.md — Gestão Acervo

> **Projeto:** Gestão Acervo  
> **Backend:** Acervo.API (.NET 8 — Minimal API)  
> **Frontend:** acervo-web (Angular 17)  
> **Versão do Documento:** 1.0.0  
> **Data:** 2025-03-04

---

## 1. Visão Geral da Arquitetura

O projeto adota uma arquitetura em camadas inspirada nos princípios da **Clean Architecture**, separando claramente as responsabilidades entre domínio, infraestrutura e interface de entrada (API). No frontend, o padrão **Feature-based Architecture** com **NgRx** garante previsibilidade de estado e escalabilidade.

```
┌─────────────────────────────────────────────┐
│              acervo-web              │
│         Angular 17 SPA  +  NgRx             │
└────────────────────┬────────────────────────┘
                     │ HTTP/REST (JSON)
┌────────────────────▼────────────────────────┐
│              Acervo.API (Presentation)      │
│    .NET 8 Minimal API  —  v1 Endpoints      │
│         Swagger │ AppSettings │ DI           │
└────────────────────┬────────────────────────┘
                     │ chama
┌────────────────────▼────────────────────────┐
│          Acervo.Application (Use Cases)     │
│   Services │ DTOs │ ViewModels │ Validators  │
│         Mapster Configs                     │
└────────────────────┬────────────────────────┘
           chama     │     implementa interfaces
     ┌───────────────┴──────────────────┐
     ▼                                  ▼
┌────────────────┐         ┌────────────────────────┐
│ Acervo.Domain  │         │ Acervo.Infrastructure  │
│   (Entities)   │◀────────│  EF Core │ Repositories│
│   Interfaces   │         │  Migrations │ DbContext │
└────────────────┘         └────────────────────────┘
                                        │ EF Core
                           ┌────────────▼───────────┐
                           │    SQL Server Database  │
                           │  Gerenciado via EF Mig. │
                           └────────────────────────┘
```

---

## 2. Padrões de Design e Justificativas

### 2.1 Clean Architecture

**Decisão:** Organizar o backend em **quatro camadas distintas** — `Domain`, `Application`, `Infrastructure` e `API`.

**Justificativa:**
- **Isolamento do domínio:** As entidades e regras de negócio (`Domain`) não conhecem EF Core, HTTP ou qualquer framework externo. Isso garante que mudanças de infraestrutura não impactem a lógica central.
- **Camada de aplicação dedicada:** O projeto `Acervo.Application` centraliza toda a orquestração de casos de uso — Services, DTOs, ViewModels, Validators e Mappings — sem depender de detalhes de infraestrutura ou de HTTP. Isso evita que a camada `API` acumule responsabilidades além do roteamento e serialização.
- **Testabilidade:** A separação entre `Application` e `Infrastructure` permite testar os Services com repositórios mockados (Moq) sem nenhuma dependência de banco de dados ou contexto HTTP.
- **Aderência ao desafio:** Atende diretamente ao critério *"Separação adequada de responsabilidades"* e *"Modelagem correta do domínio"*.

```
Acervo.API (solution)
├── Acervo.Domain         → Entidades, Interfaces de Repositório, Result pattern
├── Acervo.Application    → Services, DTOs, ViewModels, Validators, Mapster Configs
├── Acervo.Infrastructure → DbContext, Implementação dos Repositórios, Migrations
└── Acervo.API            → Endpoints (Minimal API), configuração do pipeline, AppSettings
```

**Fluxo de dependências (regra de ouro da Clean Architecture):**

```
Acervo.API  →  Acervo.Application  →  Acervo.Domain
                                   ↑
                    Acervo.Infrastructure
```

> Nenhuma camada interna conhece a camada que a referencia. O `Domain` não conhece ninguém. A `Infrastructure` conhece apenas o `Domain` (implementa suas interfaces). A `Application` conhece apenas o `Domain`. A `API` conhece `Application` e `Infrastructure` (apenas para registro de DI no `Program.cs`).

---

### 2.2 Repository Pattern

**Decisão:** Implementar interfaces de repositório (`IAutorRepository`, `IGeneroRepository`, `ILivroRepository`) no `Domain`, com implementações concretas no `Infrastructure`.

**Justificativa:**
- Embora o EF Core já seja uma abstração sobre o banco, o Repository Pattern adiciona uma **fronteira explícita** entre a lógica de negócio e a persistência.
- Permite **mockar** as dependências nos testes de unidade com `Moq`, sem necessidade de banco em memória.
- Facilita a troca de ORM ou estratégia de persistência no futuro sem alterar os serviços.

```csharp
// Domain/Interfaces
public interface ILivroRepository
{
    Task<Livro?> GetByIdAsync(Guid id);
    Task<bool> IsbnExisteAsync(string isbn, Guid? ignorarId = null);
    Task AddAsync(Livro livro);
    // ...
}

// Infrastructure/Repositories
public class LivroRepository : ILivroRepository
{
    private readonly AcervoDbContext _context;
    public LivroRepository(AcervoDbContext context) => _context = context;
    // implementação com EF Core
}
```

---

### 2.3 Dependency Injection (DI)

**Decisão:** Utilizar o container nativo do .NET (`IServiceCollection`) para registrar todas as dependências.

**Justificativa:**
- Princípio da **Inversão de Dependência (DIP)** — módulos de alto nível não dependem de implementações concretas.
- O registro é dividido em dois extension methods, um por camada, mantendo o `Program.cs` limpo e legível.
- Facilita a substituição de implementações para testes ou evolução tecnológica.

```csharp
// Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAutorService, AutorService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ILivroService, LivroService>();
        services.AddMapster();
        services.AddValidatorsFromAssemblyContaining<CreateAutorDtoValidator>();
        return services;
    }
}

// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AcervoDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ILivroRepository, LivroRepository>();

        return services;
    }
}

// API/Program.cs
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
```

---

### 2.4 DTOs e ViewModels

**Decisão:** Separar os contratos da API das entidades de domínio com **DTOs** (entrada) e **ViewModels** (saída), todos residindo na camada `Acervo.Application`.

**Justificativa:**
- Evita a exposição direta das entidades (prevenindo over-posting e vazamento de dados internos).
- Permite que o contrato da API evolua independentemente do modelo de domínio.
- Centralizar DTOs e ViewModels na `Application` garante que tanto a `API` quanto futuros consumers (ex: worker services) reutilizem os mesmos contratos.
- Mapear com **Mapster** (também na `Application`) elimina código repetitivo de conversão manual, com ganho de performance por não usar reflection em runtime.

| Camada | Tipo | Direção |
|--------|------|---------|
| `CreateAutorDto` | DTO | Request → Service |
| `UpdateLivroDto` | DTO | Request → Service |
| `AutorViewModel` | ViewModel | Service → Response |
| `LivroDetalheViewModel` | ViewModel | Service → Response (com dados do Autor e Gênero) |

---

## 3. Persistence Layer — SQL Server + EF Core

### 3.1 Estratégia de Persistência

**Decisão:** Entity Framework Core 8 como ORM principal com SQL Server.

**Justificativa:**
- EF Core é o ORM padrão e mais maduro do ecossistema .NET, com suporte nativo a Migrations, LINQ e relacionamentos.
- O uso de **Fluent API** no `OnModelCreating` permite configuração granular sem poluir as entidades com atributos.
- **Code-First com Migrations** garante que o esquema do banco seja parte do código-fonte, versionável e rastreável.

### 3.2 Configuração das Entidades (Fluent API)

```csharp
// Infrastructure/Data/Configurations/LivroConfiguration.cs
public class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Titulo)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(l => l.ISBN)
               .IsRequired()
               .HasMaxLength(13);

        builder.HasIndex(l => l.ISBN)
               .IsUnique();

        builder.HasOne(l => l.Autor)
               .WithMany(a => a.Livros)
               .HasForeignKey(l => l.AutorId)
               .OnDelete(DeleteBehavior.Restrict); // RN-07

        builder.HasOne(l => l.Genero)
               .WithMany(g => g.Livros)
               .HasForeignKey(l => l.GeneroId)
               .OnDelete(DeleteBehavior.Restrict); // RN-08
    }
}
```

> **Nota:** `DeleteBehavior.Restrict` implementa diretamente as regras RN-07 e RN-08, impedindo deleção de Autor/Gênero com vínculos a nível de banco de dados, complementado pela validação no service layer.

### 3.3 Workflow de Migrations

```bash
# Criar nova migration
dotnet ef migrations add <NomeDaMigration> --project Acervo.Infrastructure --startup-project Acervo.API

# Aplicar ao banco
dotnet ef database update --project Acervo.Infrastructure --startup-project Acervo.API
```

---

## 4. Backend — Acervo.API (.NET 8 Minimal API)

### 4.1 Minimal API vs Controller-Based

**Decisão:** Utilizar Minimal API.

**Justificativa:**
- Menor cerimônia de código (sem classes de controller, sem atributos de rota espalhados).
- Performance superior — menos middleware na pipeline de request.
- Permite organizar endpoints em **Extension Methods** por entidade, mantendo o `Program.cs` enxuto.

```csharp
// API/Endpoints/AutorEndpoints.cs
public static class AutorEndpoints
{
    public static void MapAutorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/autores")
                       .WithTags("Autores")
                       .WithOpenApi();

        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:guid}", UpdateAsync);
        group.MapDelete("/{id:guid}", DeleteAsync);
    }
}
```

### 4.2 Versionamento de Rotas

**Decisão:** Versionamento via prefixo de rota `/api/v{version}/`.

**Justificativa:**
- Abordagem simples, explícita e amplamente adotada — qualquer cliente sabe exatamente qual versão está consumindo.
- Não exige bibliotecas adicionais para o escopo do desafio, mas é compatível com `Asp.Versioning` para evolução futura.

### 4.3 FluentValidation

**Decisão:** Validar DTOs de entrada com FluentValidation, integrado ao pipeline do .NET via `AddFluentValidationAutoValidation()`.

**Justificativa:**
- Separação da lógica de validação das entidades de domínio e dos endpoints.
- Mensagens de erro ricas e centralizadas, retornadas automaticamente como `400 Bad Request`.

```csharp
// API/Validators/CreateLivroValidator.cs
public class CreateLivroDtoValidator : AbstractValidator<CreateLivroDto>
{
    public CreateLivroDtoValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ISBN).NotEmpty().Length(10, 13);
        RuleFor(x => x.AnoPublicacao)
            .InclusiveBetween(1400, DateTime.Now.Year);
        RuleFor(x => x.AutorId).NotEmpty();
        RuleFor(x => x.GeneroId).NotEmpty();
    }
}
```

### 4.4 Mapster

**Decisão:** Usar **Mapster** para mapear entidades ↔ DTOs/ViewModels, via **TypeAdapterConfig** centralizado.

**Justificativa:**
- Mapster é significativamente mais performático que AutoMapper — gera código de mapeamento em tempo de compilação (source generation) sem reflection em runtime.
- API fluente e concisa, sem necessidade de classes `Profile` separadas.
- Suporte nativo a mapeamento de records, structs e objetos imutáveis.

```csharp
// Application/Mappings/AutorMappingConfig.cs
public class AutorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Autor, AutorViewModel>();
        config.NewConfig<CreateAutorDto, Autor>();
        config.NewConfig<UpdateAutorDto, Autor>();
    }
}

// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    var config = TypeAdapterConfig.GlobalSettings;
    config.Scan(Assembly.GetExecutingAssembly()); // registra todos os IRegister
    services.AddSingleton(config);
    services.AddScoped<IMapper, ServiceMapper>();
    // ...
    return services;
}
```

---

## 5. Frontend — acervo-web (Angular 17)

### 5.1 Arquitetura Feature-Based

**Decisão:** Organizar o projeto por **features** (autores, generos, livros), cada uma com seus próprios componentes, services, store e models.

**Justificativa:**
- Alta coesão dentro da feature, baixo acoplamento entre features.
- Facilita lazy loading por módulo/rota.
- Escala bem conforme o projeto cresce.

### 5.2 Ciclo de Vida dos Componentes

Os componentes seguem o padrão **Smart/Dumb (Container/Presentational)**:

| Tipo | Responsabilidade |
|------|-----------------|
| **Smart Component** | Interage com o Store (NgRx), dispara actions, recebe dados via selectors |
| **Dumb Component** | Recebe dados via `@Input()`, emite eventos via `@Output()`, sem lógica de negócio |

```typescript
// Smart: livros-list.component.ts
@Component({ ... })
export class LivrosListComponent implements OnInit {
  livros$ = this.store.select(selectAllLivros);

  constructor(private store: Store) {}

  ngOnInit() {
    this.store.dispatch(LivrosActions.loadLivros());
  }

  onDelete(id: string) {
    this.store.dispatch(LivrosActions.deleteLivro({ id }));
  }
}
```

### 5.3 NgRx — Gerenciamento de Estado

**Decisão:** NgRx com o padrão completo: Actions → Reducer → Effects → Selectors.

**Justificativa:**
- Estado global previsível e imutável — qualquer mudança é rastreável via Redux DevTools.
- **Effects** isolam as chamadas HTTP (side effects) dos reducers puros.
- Evita prop drilling e sincronização manual entre componentes.

```
Componente → dispatch(Action) → Effect (chamada HTTP) → dispatch(SuccessAction) → Reducer → State → Selector → Componente
```

### 5.4 Services

```typescript
// features/livros/services/livro.service.ts
@Injectable({ providedIn: 'root' })
export class LivroService {
  private apiUrl = `${environment.apiUrl}/v1/livros`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<LivroViewModel[]>> {
    return this.http.get<ApiResponse<LivroViewModel[]>>(this.apiUrl);
  }

  create(dto: CreateLivroDto): Observable<ApiResponse<LivroViewModel>> {
    return this.http.post<ApiResponse<LivroViewModel>>(this.apiUrl, dto);
  }
}
```

### 5.5 HTTP Interceptor

**Decisão:** Implementar um `ErrorInterceptor` para tratamento centralizado de erros HTTP.

**Justificativa:**
- Evita duplicação de lógica de tratamento de erro em cada service.
- Ponto único para exibir notificações de erro (toast/snackbar) e, futuramente, para injetar tokens de autenticação.

```typescript
// core/interceptors/error.interceptor.ts
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = error.error?.message ?? 'Erro inesperado.';
      // dispatch para notificação global
      return throwError(() => error);
    })
  );
};
```

### 5.6 Environments

```typescript
// environments/environment.ts (Development)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};

// environments/environment.prod.ts (Production)
export const environment = {
  production: true,
  apiUrl: 'https://api.gestaoacervo.com/api'
};
```

---

## 6. Estratégia de Qualidade e Testes

### 6.1 Backend — xUnit + Moq

**Decisão:** Testes de unidade com xUnit e Moq, seguindo o padrão **Arrange / Act / Assert**.

**O que será testado:**
- Serviços/use cases: validações de negócio (ISBN duplicado, e-mail duplicado, exclusão com vínculo).
- Repositórios: mockados com `Moq` para isolar o banco de dados.
- Validators: FluentValidation testado com `TestValidate`.
- Asserções com sintaxe legível via **Shouldly** (ex: `result.ShouldBeTrue()`, `value.ShouldContain(...)`).

```csharp
// Tests/Services/LivroServiceTests.cs
public class LivroServiceTests
{
    [Fact]
    public async Task CriarLivro_IsbnDuplicado_DeveRetornarConflict()
    {
        // Arrange
        var repoMock = new Mock<ILivroRepository>();
        repoMock.Setup(r => r.IsbnExisteAsync("978-0-000000-00-0", null))
                .ReturnsAsync(true);

        var service = new LivroService(repoMock.Object, /* outros mocks */);

        // Act
        var result = await service.CriarAsync(new CreateLivroDto { ISBN = "978-0-000000-00-0" });

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("ISBN já cadastrado");
    }
}
```

### 6.2 Frontend — Jasmine + Karma

**O que será testado:**
- **Services:** Verificar chamadas HTTP corretas com `HttpClientTestingModule`.
- **Reducers:** Testar transições de estado puras (dado estado inicial + action → novo estado esperado).
- **Effects:** Testar com `provideMockActions` e `provideMockStore`.
- **Components (Smart):** Verificar dispatch de actions no `ngOnInit` e em interações do usuário.

```typescript
// features/livros/store/livros.reducer.spec.ts
describe('LivrosReducer', () => {
  it('deve adicionar livros ao estado ao carregar com sucesso', () => {
    const livros = [{ id: '1', titulo: 'Clean Code' }] as LivroViewModel[];
    const action = LivrosActions.loadLivrosSuccess({ livros });
    const state = livrosReducer(initialState, action);

    expect(state.livros).toEqual(livros);
    expect(state.loading).toBeFalse();
  });
});
```

---

## 7. Decisões de Segurança e Integridade

| Decisão | Justificativa |
|---------|---------------|
| `DeleteBehavior.Restrict` no EF Core | Garante integridade referencial a nível de banco, independente do service layer |
| Validação em duas camadas (FluentValidation + Service) | Defense in depth — a validação de unicidade exige consulta ao banco e não pode ser feita apenas no validator |
| Resposta padronizada com envelope | Garante contrato estável para o frontend independente do tipo de erro |
| Migrations versionadas no Git | Rastreabilidade completa do esquema do banco ao longo do tempo |

---

*Documento gerado para o Desafio Técnico — Siemens | Projeto Gestão Acervo*
