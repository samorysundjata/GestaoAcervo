namespace Acervo.Application.DTOs.Livro;

public record UpdateLivroDto(string Titulo, string ISBN, int AnoPublicacao, Guid AutorId, Guid GeneroId);
