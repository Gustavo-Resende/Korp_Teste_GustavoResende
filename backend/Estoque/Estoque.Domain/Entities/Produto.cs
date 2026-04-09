using Korp.Shared;

namespace Estoque.Domain.Entities;

public class Produto
{
    public int Id { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public int Saldo { get; private set; }

    private Produto()
    {
    }

    public static Result<Produto> Criar(string codigo, string descricao, int saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return Result<Produto>.Failure("Código é obrigatório.");
        if (string.IsNullOrWhiteSpace(descricao))
            return Result<Produto>.Failure("Descrição é obrigatória.");
        if (saldoInicial < 0)
            return Result<Produto>.Failure("Saldo inicial não pode ser negativo.");

        var p = new Produto
        {
            Codigo = codigo.Trim(),
            Descricao = descricao.Trim(),
            Saldo = saldoInicial
        };
        return Result<Produto>.Success(p);
    }

    public Result DeduzirSaldo(int quantidade)
    {
        if (quantidade <= 0)
            return Result.Failure("Quantidade deve ser maior que zero.");
        if (quantidade > Saldo)
            return Result.Failure($"Saldo insuficiente para o produto {Codigo}.");

        Saldo -= quantidade;
        return Result.Success();
    }
}
