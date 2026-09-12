using Acervo.Domain.Entities;
using Xunit;

namespace Acervo.Domain.Tests.Entities;

public class GeneroTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesAndCollections()
    {
        var genero = new Genero("Poesia");

        Assert.NotEqual(Guid.Empty, genero.Id);
        Assert.Equal("Poesia", genero.Nome);
        Assert.NotNull(genero.Livros);
        Assert.Empty(genero.Livros);
    }

    [Fact]
    public void Update_ShouldChangeNome_KeepIdAndCollections()
    {
        var genero = new Genero("Poesia");
        var originalId = genero.Id;

        genero.Update("Romance");

        Assert.Equal(originalId, genero.Id);
        Assert.Equal("Romance", genero.Nome);
        Assert.NotNull(genero.Livros);
        Assert.Empty(genero.Livros);
    }

    [Fact]
    public void EfConstructor_ShouldInitializeDefaults()
    {
        var genero = (Genero)Activator.CreateInstance(typeof(Genero), nonPublic: true)!;

        Assert.Equal(Guid.Empty, genero.Id);
        Assert.Equal(string.Empty, genero.Nome);
        Assert.NotNull(genero.Livros);
        Assert.Empty(genero.Livros);
    }
}
