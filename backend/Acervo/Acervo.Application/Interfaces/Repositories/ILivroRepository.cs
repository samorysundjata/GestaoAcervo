using Acervo.Domain.Entities;

namespace Acervo.Application.Interfaces.Repositories;

public interface ILivroRepository
{
    Task<IEnumerable<Livro>> GetAllAsync();
    Task<Livro?> GetByIdAsync(Guid id);
    Task<bool> IsbnExistsAsync(string isbn, Guid? ignoreId = null);
    Task AddAsync(Livro livro);
    Task UpdateAsync(Livro livro);
    Task DeleteAsync(Livro livro);
}