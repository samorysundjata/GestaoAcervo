namespace Acervo.Application.ViewModels;

public record LivroViewModel(Guid Id, string Titulo, string ISBN, int AnoPublicacao, Guid AutorId, Guid GeneroId);
