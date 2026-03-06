namespace Acervo.Domain.Entities;

public class Genero
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public ICollection<Livro> Livros { get; private set; } = new List<Livro>();

    protected Genero() { }

    public Genero(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome;
    }

    public void Update(string nome) => Nome = nome;
}
