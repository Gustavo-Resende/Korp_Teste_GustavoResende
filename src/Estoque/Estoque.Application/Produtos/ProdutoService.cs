using Estoque.Application.Abstractions;
using Estoque.Domain.Entities;
using FluentValidation;
using Korp.Shared;

namespace Estoque.Application.Produtos;

public class ProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IValidator<CriarProdutoCommand> _validator;

    public ProdutoService(IProdutoRepository repository, IValidator<CriarProdutoCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<Produto>> CriarAsync(CriarProdutoCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result<Produto>.Failure(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        if (await _repository.ExistsByCodigoAsync(command.Codigo, cancellationToken))
            return Result<Produto>.Failure("Já existe produto com este código.");

        var criado = Produto.Criar(command.Codigo, command.Descricao, command.SaldoInicial);
        if (!criado.IsSuccess)
            return criado;

        await _repository.AddAsync(criado.Value!, cancellationToken);
        return Result<Produto>.Success(criado.Value!);
    }

    public Task<Produto?> ObterPorIdAsync(int id, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken) =>
        _repository.ListarAsync(cancellationToken);

    public Task<Result> DeduzirItensAsync(
        IReadOnlyList<(int ProdutoId, int Quantidade)> itens,
        CancellationToken cancellationToken) =>
        _repository.DeduzirItensEmTransacaoAsync(itens, cancellationToken);
}
