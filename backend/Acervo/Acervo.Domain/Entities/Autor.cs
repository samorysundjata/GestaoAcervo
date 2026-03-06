namespace Acervo.Domain.Entities;

public class Autor
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public ICollection<Livro> Livros { get; private set; } = new List<Livro>();

    protected Autor() { }

    public Autor(string nome, string email)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
    }

    public void Update(string nome, string email)
    {
        Nome = nome;
        Email = email;
    }
}
