using System.Net.Http.Json;
using Faturamento.Application.Abstractions;
using Korp.Shared;
using Microsoft.Extensions.Logging;

namespace Faturamento.Infrastructure.Http;

public class EstoqueApiClient : IEstoqueApiClient
{
    public const string HttpClientName = "EstoqueApi";
    private const string IndisponivelPrefixo = "UNAVAILABLE:";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EstoqueApiClient> _logger;

    public EstoqueApiClient(IHttpClientFactory httpClientFactory, ILogger<EstoqueApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result> DeduzirItensAsync(
        IReadOnlyList<(int ProdutoId, int Quantidade)> itens,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);
        var payload = new
        {
            Itens = itens.Select(i => new { ProdutoId = i.ProdutoId, Quantidade = i.Quantidade }).ToList()
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/Produtos/deduzir", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
                return Result.Success();

            var texto = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result.Failure($"Saldo ou validação no estoque: {texto}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Falha de rede ao chamar API de estoque");
            return Result.Failure($"{IndisponivelPrefixo}Serviço de estoque indisponível.");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout ao chamar API de estoque");
            return Result.Failure($"{IndisponivelPrefixo}Tempo esgotado ao contatar o estoque.");
        }
    }
}
