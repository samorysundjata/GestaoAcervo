using Acervo.Application.DTOs.Genero;
using FluentValidation;

namespace Acervo.Application.Validators;

public class CreateGeneroValidator : AbstractValidator<CreateGeneroDto>
{
    public CreateGeneroValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");
    }
}
