using Faturamento.Domain.Entities;
using Faturamento.Domain.Enums;
using FluentAssertions;

namespace Korp.Tests.Domain;

public class NotaFiscalDomainTests
{
    [Fact]
    public void Criar_DeveIniciarComoAberta()
    {
        var nota = NotaFiscal.Criar(1);
        nota.Status.Should().Be(StatusNotaFiscal.Aberta);
        nota.Numero.Should().Be(1);
    }

    [Fact]
    public void FecharParaImpressao_QuandoAbertaComItens_DeveFicarFechada()
    {
        var nota = NotaFiscal.Criar(1);
        nota.AdicionarItem(10, 2).IsSuccess.Should().BeTrue();
        var r = nota.FecharParaImpressao();
        r.IsSuccess.Should().BeTrue();
        nota.Status.Should().Be(StatusNotaFiscal.Fechada);
    }

    [Fact]
    public void FecharParaImpressao_QuandoJaFechada_DeveRetornarFalha()
    {
        var nota = NotaFiscal.Criar(1);
        nota.AdicionarItem(1, 1);
        nota.FecharParaImpressao().IsSuccess.Should().BeTrue();
        var r = nota.FecharParaImpressao();
        r.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void FecharParaImpressao_QuandoSemItens_DeveRetornarFalha()
    {
        var nota = NotaFiscal.Criar(1);
        var r = nota.FecharParaImpressao();
        r.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void AdicionarItem_QuandoNotaFechada_DeveRetornarFalha()
    {
        var nota = NotaFiscal.Criar(1);
        nota.AdicionarItem(1, 1);
        nota.FecharParaImpressao();
        var r = nota.AdicionarItem(2, 1);
        r.IsSuccess.Should().BeFalse();
    }
}
