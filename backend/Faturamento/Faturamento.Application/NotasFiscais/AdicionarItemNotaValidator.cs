using FluentValidation;

namespace Faturamento.Application.NotasFiscais;

public class AdicionarItemNotaValidator : AbstractValidator<AdicionarItemNotaCommand>
{
    public AdicionarItemNotaValidator()
    {
        RuleFor(x => x.NotaFiscalId).GreaterThan(0);
        RuleFor(x => x.ProdutoId).GreaterThan(0);
        RuleFor(x => x.Quantidade).GreaterThan(0);
    }
}
