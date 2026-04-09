using Faturamento.Application.Abstractions;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Enums;
using FluentValidation;
using Korp.Shared;
using Microsoft.Extensions.Logging;

namespace Faturamento.Application.NotasFiscais;

public class NotaFiscalService
{
    public const string PrefixoIndisponivel = "UNAVAILABLE:";

    private readonly INotaFiscalRepository _repository;
    private readonly IEstoqueApiClient _estoqueApiClient;
    private readonly IValidator<AdicionarItemNotaCommand> _adicionarItemValidator;
    private readonly ILogger<NotaFiscalService> _logger;

    public NotaFiscalService(
        INotaFiscalRepository repository,
        IEstoqueApiClient estoqueApiClient,
        IValidator<AdicionarItemNotaCommand> adicionarItemValidator,
        ILogger<NotaFiscalService> logger)
    {
        _repository = repository;
        _estoqueApiClient = estoqueApiClient;
        _adicionarItemValidator = adicionarItemValidator;
        _logger = logger;
    }

    public async Task<Result<int>> CriarNotaAsync(CancellationToken cancellationToken)
    {
        var numero = await _repository.ObterProximoNumeroAsync(cancellationToken);
        var nota = NotaFiscal.Criar(numero);
        await _repository.AddAsync(nota, cancellationToken);
        return Result<int>.Success(nota.Id);
    }

    public async Task<Result> AdicionarItemAsync(
        AdicionarItemNotaCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await _adicionarItemValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var nota = await _repository.GetByIdComItensTrackedAsync(command.NotaFiscalId, cancellationToken);
        if (nota is null)
            return Result.Failure("Nota fiscal não encontrada.");

        var adicionar = nota.AdicionarItem(command.ProdutoId, command.Quantidade);
        if (!adicionar.IsSuccess)
            return adicionar;

        await _repository.UpdateAsync(nota, cancellationToken);
        return Result.Success();
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(int id, CancellationToken cancellationToken) =>
        await _repository.GetByIdComItensAsync(id, cancellationToken);

    public async Task<IReadOnlyList<NotaFiscal>> ListarAsync(CancellationToken cancellationToken) =>
        await _repository.ListarAsync(cancellationToken);

    public async Task<Result> ImprimirAsync(int notaFiscalId, CancellationToken cancellationToken)
    {
        var nota = await _repository.GetByIdComItensTrackedAsync(notaFiscalId, cancellationToken);
        if (nota is null)
            return Result.Failure("Nota fiscal não encontrada.");

        if (nota.Status != StatusNotaFiscal.Aberta)
            return Result.Failure("Apenas notas abertas podem ser impressas.");
        if (nota.Itens.Count == 0)
            return Result.Failure("Nota sem itens não pode ser impressa.");

        var itensDeducao = nota.Itens.Select(i => (i.ProdutoId, i.Quantidade)).ToList();
        var deducao = await _estoqueApiClient.DeduzirItensAsync(itensDeducao, cancellationToken);
        if (!deducao.IsSuccess)
        {
            if (deducao.Error?.StartsWith(PrefixoIndisponivel, StringComparison.Ordinal) == true)
                _logger.LogWarning("Falha de comunicação com Estoque ao imprimir nota {NotaId}", notaFiscalId);
            return deducao;
        }

        var fechar = nota.FecharParaImpressao();
        if (!fechar.IsSuccess)
            return fechar;

        await _repository.UpdateAsync(nota, cancellationToken);
        return Result.Success();
    }
}
