using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estoque.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(64).IsRequired();
        builder.Property(p => p.Descricao).HasColumnName("descricao").HasMaxLength(512).IsRequired();
        builder.Property(p => p.Saldo).HasColumnName("saldo");

        builder.HasIndex(p => p.Codigo).IsUnique();
    }
}
