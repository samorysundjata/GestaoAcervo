using Acervo.Domain.Entities;
using Acervo.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Acervo.Infrastructure.Data;

public class AcervoDbContext : DbContext
{
    public AcervoDbContext(DbContextOptions<AcervoDbContext> options) : base(options) { }

    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AutorConfiguration());
        modelBuilder.ApplyConfiguration(new GeneroConfiguration());
        modelBuilder.ApplyConfiguration(new LivroConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
