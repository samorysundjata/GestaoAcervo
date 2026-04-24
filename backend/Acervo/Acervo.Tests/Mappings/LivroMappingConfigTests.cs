using System.Reflection;
using Acervo.Application.Mappings;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;
using MapsterMapper;
using Shouldly;

namespace Acervo.Tests.Mappings;

public class LivroMappingConfigTests
{
    [Fact]
    public void Register_DeveMapearLivroParaLivroViewModel()
    {
        var config = new TypeAdapterConfig();
        new LivroMappingConfig().Register(config);
        var mapper = new Mapper(config);

        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = mapper.Map<LivroViewModel>(livro);

        result.ShouldBe(new LivroViewModel(livro.Id, livro.Titulo, livro.ISBN, livro.AnoPublicacao, livro.AutorId, livro.GeneroId));
    }

    [Fact]
    public void Register_DeveMapearLivroParaLivroDetalheViewModel()
    {
        var config = new TypeAdapterConfig();
        new LivroMappingConfig().Register(config);
        var mapper = new Mapper(config);

        var autor = new Autor("Autor", "autor@dominio.com");
        var genero = new Genero("Tecnologia");
        var livro = new Livro("Clean Code", "9780132350884", 2008, autor.Id, genero.Id);

        SetPrivateProperty(livro, "Autor", autor);
        SetPrivateProperty(livro, "Genero", genero);

        var result = mapper.Map<LivroDetalheViewModel>(livro);

        result.ShouldBe(new LivroDetalheViewModel(
            livro.Id,
            livro.Titulo,
            livro.ISBN,
            livro.AnoPublicacao,
            livro.AutorId,
            autor.Nome,
            livro.GeneroId,
            genero.Nome));
    }

    private static void SetPrivateProperty<T>(T target, string propertyName, object? value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        property.SetValue(target, value);
    }
}