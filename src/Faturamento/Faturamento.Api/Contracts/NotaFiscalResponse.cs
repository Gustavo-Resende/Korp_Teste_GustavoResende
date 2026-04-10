namespace Faturamento.Api.Contracts;

public record ItemNotaResponse(int ProdutoId, int Quantidade);

public record NotaFiscalResponse(int Id, long Numero, string Status, IReadOnlyList<ItemNotaResponse> Itens);
