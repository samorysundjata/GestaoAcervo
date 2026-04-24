using Acervo.Application.DTOs.Genero;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace Acervo.Tests.Validators;

public class UpdateGeneroValidatorTests
{
    private readonly UpdateGeneroValidator _validator = new();

    [Fact]
    public void NomeVazio_DeveRetornarErro()
    {
        var dto = new UpdateGeneroDto("");
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void NomeMuitoLongo_DeveRetornarErro()
    {
        var dto = new UpdateGeneroDto(new string('a', 101));
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void DadosValidos_NaoDeveRetornarErro()
    {
        var dto = new UpdateGeneroDto("Tecnologia");
        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}