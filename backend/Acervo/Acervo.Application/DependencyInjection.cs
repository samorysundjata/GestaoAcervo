using System.Reflection;
using Acervo.Application.Services;
using Acervo.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Mapster;
using MapsterMapper;
using FluentValidation;

namespace Acervo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<TypeAdapterConfig>()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<IAutorService, AutorService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ILivroService, LivroService>();

        return services;
    }
}
