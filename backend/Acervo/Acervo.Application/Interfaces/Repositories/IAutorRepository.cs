using Acervo.Domain.Entities;

namespace Acervo.Application.Interfaces.Repositories;

public interface IAutorRepository
{
    Task<IEnumerable<Autor>> GetAllAsync();
    Task<Autor?> GetByIdAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? ignoreId = null);
    Task<bool> HasLivrosAsync(Guid id);
    Task AddAsync(Autor autor);
    Task UpdateAsync(Autor autor);
    Task DeleteAsync(Autor autor);
}
