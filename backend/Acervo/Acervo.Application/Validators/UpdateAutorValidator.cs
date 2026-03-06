using Acervo.Application.DTOs.Autor;
using FluentValidation;

namespace Acervo.Application.Validators;

public class UpdateAutorValidator : AbstractValidator<UpdateAutorDto>
{
    public UpdateAutorValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("E-mail deve ter no máximo 200 caracteres.");
    }
}
