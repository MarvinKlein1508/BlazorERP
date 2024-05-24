using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class CostCenterValidator : AbstractValidator<CostCenter>
{
    public CostCenterValidator()
    {
        RuleFor(x => x.Nummer)
            .GreaterThan(0)
            .WithMessage("Bitte geben Sie eine Nummer an");

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

    }
}
