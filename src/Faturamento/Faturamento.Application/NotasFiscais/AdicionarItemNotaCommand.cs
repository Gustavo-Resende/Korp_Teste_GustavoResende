namespace Faturamento.Application.NotasFiscais;

public record AdicionarItemNotaCommand(int NotaFiscalId, int ProdutoId, int Quantidade);
