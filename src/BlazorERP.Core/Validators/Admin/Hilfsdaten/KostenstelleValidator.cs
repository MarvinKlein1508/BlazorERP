using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class KostenstelleValidator : AbstractValidator<Kostenstelle>
{
    public KostenstelleValidator()
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
