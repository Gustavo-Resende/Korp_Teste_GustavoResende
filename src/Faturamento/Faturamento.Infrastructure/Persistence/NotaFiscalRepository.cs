using System.Data;
using Faturamento.Application.Abstractions;
using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Persistence;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly AppDbContext _db;

    public NotaFiscalRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<long> ObterProximoNumeroAsync(CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT nextval('nota_fiscal_numero_seq');";
        var scalar = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(scalar!);
    }

    public async Task AddAsync(NotaFiscal nota, CancellationToken cancellationToken)
    {
        await _db.NotasFiscais.AddAsync(nota, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(NotaFiscal nota, CancellationToken cancellationToken)
    {
        _db.NotasFiscais.Update(nota);
        return _db.SaveChangesAsync(cancellationToken);
    }

    public Task<NotaFiscal?> GetByIdComItensTrackedAsync(int id, CancellationToken cancellationToken) =>
        _db.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public Task<NotaFiscal?> GetByIdComItensAsync(int id, CancellationToken cancellationToken) =>
        _db.NotasFiscais.AsNoTracking().Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<IReadOnlyList<NotaFiscal>> ListarAsync(CancellationToken cancellationToken) =>
        await _db.NotasFiscais.AsNoTracking().Include(n => n.Itens).OrderByDescending(n => n.Numero).ToListAsync(cancellationToken);
}
