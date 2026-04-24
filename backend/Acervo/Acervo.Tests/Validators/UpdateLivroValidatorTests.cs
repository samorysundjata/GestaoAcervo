using Acervo.Application.DTOs.Livro;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace Acervo.Tests.Validators;

public class UpdateLivroValidatorTests
{
    private readonly UpdateLivroValidator _validator = new();

    [Fact]
    public void TituloVazio_DeveRetornarErro()
    {
        var dto = new UpdateLivroDto("", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Titulo);
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void IsbnVazio_DeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void AnoPublicacaoInvalido_DeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2099, Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AnoPublicacao);
    }

    [Fact]
    public void AutorIdVazio_DeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.Empty, Guid.NewGuid());
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AutorId);
    }

    [Fact]
    public void GeneroIdVazio_DeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.Empty);
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.GeneroId);
    }

    [Fact]
    public void DadosValidos_NaoDeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}