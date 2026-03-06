namespace Acervo.Application.ViewModels;

public record LivroDetalheViewModel(
    Guid Id,
    string Titulo,
    string ISBN,
    int AnoPublicacao,
    Guid AutorId,
    string AutorNome,
    Guid GeneroId,
    string GeneroNome);
