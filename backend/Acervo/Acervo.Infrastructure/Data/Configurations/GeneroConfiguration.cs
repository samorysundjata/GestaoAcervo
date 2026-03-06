using Acervo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acervo.Infrastructure.Data.Configurations;

public class GeneroConfiguration : IEntityTypeConfiguration<Genero>
{
    public void Configure(EntityTypeBuilder<Genero> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Nome).IsRequired().HasMaxLength(100);
        builder.HasIndex(g => g.Nome).IsUnique();
    }
}
