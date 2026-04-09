using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faturamento.Infrastructure.Persistence.Configurations;

public class ItemNotaConfiguration : IEntityTypeConfiguration<ItemNota>
{
    public void Configure(EntityTypeBuilder<ItemNota> builder)
    {
        builder.ToTable("itens_nota");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.NotaFiscalId).HasColumnName("nota_fiscal_id");
        builder.Property(i => i.ProdutoId).HasColumnName("produto_id");
        builder.Property(i => i.Quantidade).HasColumnName("quantidade");
    }
}
