using Acervo.Application.Interfaces.Repositories;
using Acervo.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Acervo.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class RepositoryTests
{
    private readonly ApiFactory _factory;

    public RepositoryTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task AutorRepository_EmailExists_ShouldIgnoreCurrentId()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutorRepository>();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var autor = new Autor($"Repo {suffix}", $"repo.{suffix}@test.com");

        await repo.AddAsync(autor);

        (await repo.EmailExistsAsync(autor.Email)).ShouldBeTrue();
        (await repo.EmailExistsAsync(autor.Email, autor.Id)).ShouldBeFalse();
        (await repo.HasLivrosAsync(autor.Id)).ShouldBeFalse();
    }

    [Fact]
    public async Task LivroRepository_GetById_ShouldIncludeAutorAndGenero()
    {
        using var scope = _factory.Services.CreateScope();
        var autores = scope.ServiceProvider.GetRequiredService<IAutorRepository>();
        var generos = scope.ServiceProvider.GetRequiredService<IGeneroRepository>();
        var livros = scope.ServiceProvider.GetRequiredService<ILivroRepository>();

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var autor = new Autor($"Inc {suffix}", $"inc.{suffix}@test.com");
        var genero = new Genero($"Inc {suffix}");
        await autores.AddAsync(autor);
        await generos.AddAsync(genero);

        var isbn = $"975{suffix}123".PadRight(13, '0')[..13];
        var livro = new Livro("Included", isbn, 1999, autor.Id, genero.Id);
        await livros.AddAsync(livro);

        var loaded = await livros.GetByIdAsync(livro.Id);
        loaded.ShouldNotBeNull();
        loaded.Autor.Nome.ShouldBe(autor.Nome);
        loaded.Genero.Nome.ShouldBe(genero.Nome);
        (await autores.HasLivrosAsync(autor.Id)).ShouldBeTrue();
        (await generos.HasLivrosAsync(genero.Id)).ShouldBeTrue();
        (await livros.IsbnExistsAsync(isbn)).ShouldBeTrue();
        (await livros.IsbnExistsAsync(isbn, livro.Id)).ShouldBeFalse();
    }
}
