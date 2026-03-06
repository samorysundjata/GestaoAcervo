using Acervo.Application.DTOs.Autor;
using Acervo.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace Acervo.Tests.Validators;

public class CreateAutorValidatorTests
{
    private readonly CreateAutorValidator _validator = new();

    [Fact]
    public void NomeVazio_DeveRetornarErro()
    {
        var result = _validator.TestValidate(new CreateAutorDto("", "email@test.com"));
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void EmailInvalido_DeveRetornarErro()
    {
        var result = _validator.TestValidate(new CreateAutorDto("Nome", "email-invalido"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void DadosValidos_NaoDeveRetornarErro()
    {
        var result = _validator.TestValidate(new CreateAutorDto("Robert Martin", "uncle.bob@test.com"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
