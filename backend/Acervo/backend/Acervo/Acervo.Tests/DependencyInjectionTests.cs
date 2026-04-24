using Acervo.Application;
using Acervo.Application.DTOs.Livro;
using Acervo.Application.Interfaces.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Acervo.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_DeveRegistrarServicosEValidadores()
    {
        var services = new ServiceCollection();

        services.AddApplication();
        using var provider = services.BuildServiceProvider();

        provider.GetService<IAutorService>().ShouldNotBeNull();
        provider.GetService<IGeneroService>().ShouldNotBeNull();
        provider.GetService<ILivroService>().ShouldNotBeNull();
        provider.GetService<IMapper>().ShouldNotBeNull();
        provider.GetService<TypeAdapterConfig>().ShouldNotBeNull();
        provider.GetService<IValidator<CreateLivroDto>>().ShouldNotBeNull();
        provider.GetService<IValidator<UpdateLivroDto>>().ShouldNotBeNull();
    }
}