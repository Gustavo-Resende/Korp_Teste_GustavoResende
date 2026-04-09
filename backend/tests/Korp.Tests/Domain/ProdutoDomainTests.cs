using Estoque.Domain.Entities;
using FluentAssertions;

namespace Korp.Tests.Domain;

public class ProdutoDomainTests
{
    [Fact]
    public void Criar_QuandoDadosValidos_DeveRetornarSucesso()
    {
        // Arrange & Act
        var result = Produto.Criar("P1", "Produto teste", 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Codigo.Should().Be("P1");
        result.Value.Saldo.Should().Be(10);
    }

    [Fact]
    public void Criar_QuandoSaldoNegativo_DeveRetornarFalha()
    {
        var result = Produto.Criar("P1", "X", -1);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("negativo");
    }

    [Fact]
    public void Criar_QuandoDescricaoVazia_DeveRetornarFalha()
    {
        var result = Produto.Criar("P1", "   ", 0);
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void DeduzirSaldo_QuandoQuantidadeValida_DeveAtualizarSaldo()
    {
        var criado = Produto.Criar("P1", "X", 10);
        var p = criado.Value!;
        var r = p.DeduzirSaldo(3);
        r.IsSuccess.Should().BeTrue();
        p.Saldo.Should().Be(7);
    }

    [Fact]
    public void DeduzirSaldo_QuandoQuantidadeMaiorQueEstoque_DeveRetornarFalha()
    {
        var p = Produto.Criar("P1", "X", 2).Value!;
        var r = p.DeduzirSaldo(10);
        r.IsSuccess.Should().BeFalse();
        p.Saldo.Should().Be(2);
    }

    [Fact]
    public void DeduzirSaldo_QuandoQuantidadeZero_DeveRetornarFalha()
    {
        var p = Produto.Criar("P1", "X", 5).Value!;
        p.DeduzirSaldo(0).IsSuccess.Should().BeFalse();
    }
}
