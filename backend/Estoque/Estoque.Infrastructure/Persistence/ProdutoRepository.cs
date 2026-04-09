using System.Data;
using Estoque.Application.Abstractions;
using Estoque.Domain.Entities;
using Korp.Shared;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Persistence;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken) =>
        _db.Produtos.AnyAsync(p => p.Codigo == codigo, cancellationToken);

    public async Task AddAsync(Produto produto, CancellationToken cancellationToken)
    {
        await _db.Produtos.AddAsync(produto, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken) =>
        await _db.Produtos.AsNoTracking().OrderBy(p => p.Codigo).ToListAsync(cancellationToken);

    public async Task<Result> DeduzirItensEmTransacaoAsync(
        IReadOnlyList<(int ProdutoId, int Quantidade)> itens,
        CancellationToken cancellationToken)
    {
        if (itens.Count == 0)
            return Result.Failure("Lista de itens para dedução está vazia.");

        await using var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        foreach (var (produtoId, quantidade) in itens)
        {
            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId, cancellationToken);
            if (produto is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure($"Produto {produtoId} não encontrado.");
            }

            var ded = produto.DeduzirSaldo(quantidade);
            if (!ded.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ded;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
