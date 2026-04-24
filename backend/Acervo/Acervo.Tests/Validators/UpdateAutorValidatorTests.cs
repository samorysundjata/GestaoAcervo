using Acervo.Application.DTOs.Autor;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;

namespace Acervo.Tests.Validators;

public class UpdateAutorValidatorTests
{
    private readonly UpdateAutorValidator _validator = new();

    [Fact]
    public void NomeVazio_DeveRetornarErro()
    {
        var dto = new UpdateAutorDto("", "autor@dominio.com");
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void NomeMuitoLongo_DeveRetornarErro()
    {
        var dto = new UpdateAutorDto(new string('a', 151), "autor@dominio.com");
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void EmailVazio_DeveRetornarErro()
    {
        var dto = new UpdateAutorDto("Autor", "");
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmailInvalido_DeveRetornarErro()
    {
        var dto = new UpdateAutorDto("Autor", "email-invalido");
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmailMuitoLongo_DeveRetornarErro()
    {
        var dto = new UpdateAutorDto("Autor", new string('a', 191) + "@teste.com");
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void DadosValidos_NaoDeveRetornarErro()
    {
        var dto = new UpdateAutorDto("Autor", "autor@dominio.com");
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}