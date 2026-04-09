namespace Estoque.Api.Contracts;

public record DeduzirEstoqueRequest(IReadOnlyList<DeduzirEstoqueItemRequest> Itens);

public record DeduzirEstoqueItemRequest(int ProdutoId, int Quantidade);
