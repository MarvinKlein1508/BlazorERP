using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Code)
            .MaximumLength(3);

        RuleFor(x => x.DecimalPlaces)
            .GreaterThanOrEqualTo(0);
    }
}
