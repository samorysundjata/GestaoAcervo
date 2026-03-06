using Acervo.Application.Interfaces.Repositories;
using Acervo.Infrastructure.Data;
using Acervo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acervo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AcervoDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AcervoDbContext).Assembly.FullName)));

        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ILivroRepository, LivroRepository>();

        return services;
    }
}
