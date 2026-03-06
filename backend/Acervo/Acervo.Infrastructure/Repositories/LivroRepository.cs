using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Infrastructure.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly AcervoDbContext _context;
    public LivroRepository(AcervoDbContext context) => _context = context;

    public async Task<IEnumerable<Livro>> GetAllAsync() =>
        await _context.Livros
            .Include(l => l.Autor)
            .Include(l => l.Genero)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Livro?> GetByIdAsync(Guid id) =>
        await _context.Livros
            .Include(l => l.Autor)
            .Include(l => l.Genero)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<bool> IsbnExistsAsync(string isbn, Guid? ignoreId = null) =>
        await _context.Livros.AnyAsync(l => l.ISBN == isbn && l.Id != ignoreId);

    public async Task AddAsync(Livro livro)
    {
        await _context.Livros.AddAsync(livro);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Livro livro)
    {
        _context.Livros.Update(livro);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Livro livro)
    {
        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();
    }
}
