using Acervo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acervo.Infrastructure.Data.Configurations;

public class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(l => l.ISBN).IsRequired().HasMaxLength(13);
        builder.HasIndex(l => l.ISBN).IsUnique();
        builder.Property(l => l.AnoPublicacao).IsRequired();

        builder.HasOne(l => l.Autor)
            .WithMany(a => a.Livros)
            .HasForeignKey(l => l.AutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Genero)
            .WithMany(g => g.Livros)
            .HasForeignKey(l => l.GeneroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
