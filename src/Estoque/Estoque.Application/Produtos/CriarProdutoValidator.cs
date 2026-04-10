using FluentValidation;

namespace Estoque.Application.Produtos;

public class CriarProdutoValidator : AbstractValidator<CriarProdutoCommand>
{
    public CriarProdutoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .MaximumLength(64);
        RuleFor(x => x.Descricao)
            .NotEmpty()
            .MaximumLength(512);
        RuleFor(x => x.SaldoInicial)
            .GreaterThanOrEqualTo(0);
    }
}
