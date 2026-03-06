using Acervo.Application.DTOs.Genero;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;

namespace Acervo.Tests.Validators;

public class CreateGeneroValidatorTests
{
    private readonly CreateGeneroValidator _validator = new();

    [Fact]
    public void NomeVazio_DeveRetornarErro()
    {
        var result = _validator.TestValidate(new CreateGeneroDto(""));
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void NomeValido_NaoDeveRetornarErro()
    {
        var result = _validator.TestValidate(new CreateGeneroDto("Tecnologia"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
