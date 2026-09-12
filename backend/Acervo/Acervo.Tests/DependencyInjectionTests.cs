using Acervo.Application;
using Acervo.Application.DTOs.Autor;
using Acervo.Application.DTOs.Genero;
using Acervo.Application.DTOs.Livro;
using Acervo.Application.Interfaces.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Acervo.Tests;

// Smoke tests for `Acervo.Application.DependencyInjection.AddApplication()`.
// The Application layer only depends on repository INTERFACES (declared in Domain);
// it does not — and must not — register concrete repositories. That is the
// responsibility of `Acervo.Infrastructure`. Therefore this test verifies:
//   1. That AddApplication REGISTERS the service factories, validators, and
//      mapper (via ServiceDescriptor inspection, without activating them).
//   2. That the self-contained artifacts (IMapper, TypeAdapterConfig, and
//      validators — which have no repository dependency) can actually be
//      RESOLVED from the built provider.
//
// A previous version of this test attempted `GetService<IAutorService>()`,
// which fails at activation because `AutorService` needs an `IAutorRepository`
// that Application does not (and should not) register. That was a bug in the
// test, not in the DI wiring — the wiring itself is correct.
public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_DeveRegistrarFactoriesDeServicosDeAplicacao()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services.ShouldContain(sd => sd.ServiceType == typeof(IAutorService));
        services.ShouldContain(sd => sd.ServiceType == typeof(IGeneroService));
        services.ShouldContain(sd => sd.ServiceType == typeof(ILivroService));
    }

    [Fact]
    public void AddApplication_DeveResolverMapperEConfiguracaoDoMapster()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();

        provider.GetService<TypeAdapterConfig>().ShouldNotBeNull();
        provider.GetService<IMapper>().ShouldNotBeNull();
    }

    [Fact]
    public void AddApplication_DeveResolverValidadoresDoFluentValidation()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();

        provider.GetService<IValidator<CreateAutorDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<UpdateAutorDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<CreateGeneroDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<UpdateGeneroDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<CreateLivroDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<UpdateLivroDto>>().ShouldNotBeNull();
    }
}
