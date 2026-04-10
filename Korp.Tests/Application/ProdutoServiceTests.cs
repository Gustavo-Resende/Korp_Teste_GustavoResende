using Estoque.Application.Abstractions;
using Estoque.Application.Produtos;
using Estoque.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace Korp.Tests.Application;

public class ProdutoServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoDadosValidosEDisponiveis_DeveRetornarSucesso()
    {
        // Arrange
        var repository = Substitute.For<IProdutoRepository>();
        var validator = Substitute.For<IValidator<CriarProdutoCommand>>();
        var command = new CriarProdutoCommand("P001", "Notebook", 5);

        validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        repository.ExistsByCodigoAsync("P001", Arg.Any<CancellationToken>()).Returns(false);

        var sut = new ProdutoService(repository, validator);

        // Act
        var result = await sut.CriarAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Codigo.Should().Be("P001");
        await repository.Received(1).AddAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsync_QuandoCodigoJaExiste_DeveRetornarFalha()
    {
        var repository = Substitute.For<IProdutoRepository>();
        var validator = Substitute.For<IValidator<CriarProdutoCommand>>();
        var command = new CriarProdutoCommand("P1", "A", 1);

        validator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
        repository.ExistsByCodigoAsync("P1", Arg.Any<CancellationToken>()).Returns(true);

        var sut = new ProdutoService(repository, validator);
        var result = await sut.CriarAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("código");
        await repository.DidNotReceive().AddAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>());
    }
}
