namespace Estoque.Api.Contracts;

public record ProdutoResponse(int Id, string Codigo, string Descricao, int Saldo);
