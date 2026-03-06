namespace Acervo.Domain.Entities;

public class Livro
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string ISBN { get; private set; } = string.Empty;
    public int AnoPublicacao { get; private set; }
    public Guid AutorId { get; private set; }
    public Autor Autor { get; private set; } = null!;
    public Guid GeneroId { get; private set; }
    public Genero Genero { get; private set; } = null!;

    protected Livro() { }

    public Livro(string titulo, string isbn, int anoPublicacao, Guid autorId, Guid generoId)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        ISBN = isbn;
        AnoPublicacao = anoPublicacao;
        AutorId = autorId;
        GeneroId = generoId;
    }

    public void Update(string titulo, string isbn, int anoPublicacao, Guid autorId, Guid generoId)
    {
        Titulo = titulo;
        ISBN = isbn;
        AnoPublicacao = anoPublicacao;
        AutorId = autorId;
        GeneroId = generoId;
    }
}