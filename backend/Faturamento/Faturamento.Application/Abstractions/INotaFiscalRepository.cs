using Faturamento.Domain.Entities;

namespace Faturamento.Application.Abstractions;

public interface INotaFiscalRepository
{
    Task<long> ObterProximoNumeroAsync(CancellationToken cancellationToken);

    Task AddAsync(NotaFiscal nota, CancellationToken cancellationToken);

    Task UpdateAsync(NotaFiscal nota, CancellationToken cancellationToken);

    Task<NotaFiscal?> GetByIdComItensTrackedAsync(int id, CancellationToken cancellationToken);

    Task<NotaFiscal?> GetByIdComItensAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<NotaFiscal>> ListarAsync(CancellationToken cancellationToken);
}
