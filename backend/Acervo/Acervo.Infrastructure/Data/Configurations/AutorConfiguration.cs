using Acervo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acervo.Infrastructure.Data.Configurations;

public class AutorConfiguration : IEntityTypeConfiguration<Autor>
{
    public void Configure(EntityTypeBuilder<Autor> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Nome).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(a => a.Email).IsUnique();
    }
}
