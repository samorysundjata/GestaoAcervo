namespace Acervo.Application.DTOs.Livro;

public record CreateLivroDto(string Titulo, string ISBN, int AnoPublicacao, Guid AutorId, Guid GeneroId);
