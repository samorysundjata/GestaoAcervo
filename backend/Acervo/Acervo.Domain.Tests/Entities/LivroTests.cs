using Acervo.Domain.Entities;
using Xunit;

namespace Acervo.Domain.Tests.Entities;

public class LivroTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var autorId = Guid.NewGuid();
        var generoId = Guid.NewGuid();

        var livro = new Livro("Mensagem", "9781234567890", 1934, autorId, generoId);

        Assert.NotEqual(Guid.Empty, livro.Id);
        Assert.Equal("Mensagem", livro.Titulo);
        Assert.Equal("9781234567890", livro.ISBN);
        Assert.Equal(1934, livro.AnoPublicacao);
        Assert.Equal(autorId, livro.AutorId);
        Assert.Equal(generoId, livro.GeneroId);
    }

    [Fact]
    public void Update_ShouldChangeFields_KeepId()
    {
        var livro = new Livro("Mensagem", "9781234567890", 1934, Guid.NewGuid(), Guid.NewGuid());
        var originalId = livro.Id;
        var newAutorId = Guid.NewGuid();
        var newGeneroId = Guid.NewGuid();

        livro.Update("Orpheu", "9780987654321", 1915, newAutorId, newGeneroId);

        Assert.Equal(originalId, livro.Id);
        Assert.Equal("Orpheu", livro.Titulo);
        Assert.Equal("9780987654321", livro.ISBN);
        Assert.Equal(1915, livro.AnoPublicacao);
        Assert.Equal(newAutorId, livro.AutorId);
        Assert.Equal(newGeneroId, livro.GeneroId);
    }

    [Fact]
    public void EfConstructor_ShouldInitializeDefaults()
    {
        var livro = (Livro)Activator.CreateInstance(typeof(Livro), nonPublic: true)!;

        Assert.Equal(Guid.Empty, livro.Id);
        Assert.Equal(string.Empty, livro.Titulo);
        Assert.Equal(string.Empty, livro.ISBN);
        Assert.Equal(0, livro.AnoPublicacao);
    }
}
