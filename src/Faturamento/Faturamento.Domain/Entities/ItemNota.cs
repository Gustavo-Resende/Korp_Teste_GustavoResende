namespace Faturamento.Domain.Entities;

public class ItemNota
{
    public int Id { get; private set; }
    public int NotaFiscalId { get; private set; }
    public int ProdutoId { get; private set; }
    public int Quantidade { get; private set; }

    private ItemNota()
    {
    }

    internal static ItemNota Criar(int produtoId, int quantidade)
    {
        return new ItemNota
        {
            ProdutoId = produtoId,
            Quantidade = quantidade
        };
    }
}
