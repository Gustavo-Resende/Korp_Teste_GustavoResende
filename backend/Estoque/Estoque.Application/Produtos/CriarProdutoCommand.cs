namespace Estoque.Application.Produtos;

public record CriarProdutoCommand(string Codigo, string Descricao, int SaldoInicial);
