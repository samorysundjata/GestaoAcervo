using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Infrastructure.Repositories;

public class GeneroRepository : IGeneroRepository
{
    private readonly AcervoDbContext _context;
    public GeneroRepository(AcervoDbContext context) => _context = context;

    public async Task<IEnumerable<Genero>> GetAllAsync() =>
        await _context.Generos.AsNoTracking().ToListAsync();

    public async Task<Genero?> GetByIdAsync(Guid id) =>
        await _context.Generos.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<bool> NomeExistsAsync(string nome, Guid? ignoreId = null) =>
        await _context.Generos.AnyAsync(g => g.Nome == nome && g.Id != ignoreId);

    public async Task<bool> HasLivrosAsync(Guid id) =>
        await _context.Livros.AnyAsync(l => l.GeneroId == id);

    public async Task AddAsync(Genero genero)
    {
        await _context.Generos.AddAsync(genero);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Genero genero)
    {
        _context.Generos.Update(genero);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Genero genero)
    {
        _context.Generos.Remove(genero);
        await _context.SaveChangesAsync();
    }
}
