using Korp.Shared;

namespace Faturamento.Application.Abstractions;

public interface IEstoqueApiClient
{
    Task<Result> DeduzirItensAsync(
        IReadOnlyList<(int ProdutoId, int Quantidade)> itens,
        CancellationToken cancellationToken);
}
