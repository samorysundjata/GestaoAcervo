using Acervo.Domain.Entities;

namespace Acervo.Application.Interfaces.Repositories;

public interface IGeneroRepository
{
    Task<IEnumerable<Genero>> GetAllAsync();
    Task<Genero?> GetByIdAsync(Guid id);
    Task<bool> NomeExistsAsync(string nome, Guid? ignoreId = null);
    Task<bool> HasLivrosAsync(Guid id);
    Task AddAsync(Genero genero);
    Task UpdateAsync(Genero genero);
    Task DeleteAsync(Genero genero);
}
