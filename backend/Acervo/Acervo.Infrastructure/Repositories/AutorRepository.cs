using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Infrastructure.Repositories;

public class AutorRepository : IAutorRepository
{
    private readonly AcervoDbContext _context;
    public AutorRepository(AcervoDbContext context) => _context = context;

    public async Task<IEnumerable<Autor>> GetAllAsync() =>
        await _context.Autores.AsNoTracking().ToListAsync();

    public async Task<Autor?> GetByIdAsync(Guid id) =>
        await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<bool> EmailExistsAsync(string email, Guid? ignoreId = null) =>
        await _context.Autores.AnyAsync(a => a.Email == email && a.Id != ignoreId);

    public async Task<bool> HasLivrosAsync(Guid id) =>
        await _context.Livros.AnyAsync(l => l.AutorId == id);

    public async Task AddAsync(Autor autor)
    {
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Autor autor)
    {
        _context.Autores.Update(autor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Autor autor)
    {
        _context.Autores.Remove(autor);
        await _context.SaveChangesAsync();
    }
}
