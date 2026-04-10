using Estoque.Domain.Entities;
using Korp.Shared;

namespace Estoque.Application.Abstractions;

public interface IProdutoRepository
{
    Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken);

    Task AddAsync(Produto produto, CancellationToken cancellationToken);

    Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken);

    Task<Result> DeduzirItensEmTransacaoAsync(
        IReadOnlyList<(int ProdutoId, int Quantidade)> itens,
        CancellationToken cancellationToken);
}
