using Faturamento.Domain.Enums;
using Korp.Shared;

namespace Faturamento.Domain.Entities;

public class NotaFiscal
{
    public int Id { get; private set; }
    public long Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; }

    public List<ItemNota> Itens { get; private set; } = new();

    private NotaFiscal()
    {
    }

    public static NotaFiscal Criar(long numero)
    {
        return new NotaFiscal
        {
            Numero = numero,
            Status = StatusNotaFiscal.Aberta
        };
    }

    public Result AdicionarItem(int produtoId, int quantidade)
    {
        if (Status != StatusNotaFiscal.Aberta)
            return Result.Failure("Não é possível alterar itens de uma nota fechada.");
        if (quantidade <= 0)
            return Result.Failure("Quantidade deve ser maior que zero.");

        Itens.Add(ItemNota.Criar(produtoId, quantidade));
        return Result.Success();
    }

    public Result FecharParaImpressao()
    {
        if (Status != StatusNotaFiscal.Aberta)
            return Result.Failure("Apenas notas abertas podem ser impressas.");
        if (Itens.Count == 0)
            return Result.Failure("Nota sem itens não pode ser impressa.");

        Status = StatusNotaFiscal.Fechada;
        return Result.Success();
    }
}
