using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class ZahlungsbedingungValidator : AbstractValidator<Zahlungsbedingung>
{
    public ZahlungsbedingungValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.Nettotage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Skonto1Tage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Skonto2Tage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Skonto1Prozent)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Skonto2Prozent)
            .GreaterThanOrEqualTo(0);

    }
}
