using Acervo.Application.ViewModels;
using Shouldly;

namespace Acervo.Tests.ViewModels;

public class LivroDetalheViewModelTests
{
    [Fact]
    public void Ctor_DeveDefinirPropriedades()
    {
        var id = Guid.NewGuid();
        var autorId = Guid.NewGuid();
        var generoId = Guid.NewGuid();

        var vm = new LivroDetalheViewModel(
            id,
            "Clean Code",
            "9780132350884",
            2008,
            autorId,
            "Robert C. Martin",
            generoId,
            "Tecnologia");

        vm.Id.ShouldBe(id);
        vm.Titulo.ShouldBe("Clean Code");
        vm.ISBN.ShouldBe("9780132350884");
        vm.AnoPublicacao.ShouldBe(2008);
        vm.AutorId.ShouldBe(autorId);
        vm.AutorNome.ShouldBe("Robert C. Martin");
        vm.GeneroId.ShouldBe(generoId);
        vm.GeneroNome.ShouldBe("Tecnologia");
    }

    [Fact]
    public void Equality_DeveConsiderarTodasAsPropriedades()
    {
        var id = Guid.NewGuid();
        var autorId = Guid.NewGuid();
        var generoId = Guid.NewGuid();

        var vm1 = new LivroDetalheViewModel(id, "Clean Code", "9780132350884", 2008, autorId, "Autor", generoId, "Genero");
        var vm2 = new LivroDetalheViewModel(id, "Clean Code", "9780132350884", 2008, autorId, "Autor", generoId, "Genero");
        var vm3 = new LivroDetalheViewModel(id, "Outro", "9780132350884", 2008, autorId, "Autor", generoId, "Genero");

        vm1.ShouldBe(vm2);
        vm1.ShouldNotBe(vm3);
    }
}