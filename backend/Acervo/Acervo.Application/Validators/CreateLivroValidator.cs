using Acervo.Application.DTOs.Livro;
using FluentValidation;

namespace Acervo.Application.Validators;

public class CreateLivroValidator : AbstractValidator<CreateLivroDto>
{
    public CreateLivroValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN é obrigatório.")
            .Length(10, 13).WithMessage("ISBN deve ter entre 10 e 13 caracteres.");

        RuleFor(x => x.AnoPublicacao)
            .InclusiveBetween(1400, DateTime.UtcNow.Year)
            .WithMessage($"Ano de publicação deve estar entre 1400 e {DateTime.UtcNow.Year}.");

        RuleFor(x => x.AutorId)
            .NotEmpty().WithMessage("AutorId é obrigatório.");

        RuleFor(x => x.GeneroId)
            .NotEmpty().WithMessage("GeneroId é obrigatório.");
    }
}
