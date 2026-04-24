using Acervo.Application.DTOs.Livro;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;

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
    public void DadosValidos_NaoDeveRetornarErro()
    {
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}