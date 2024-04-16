using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class LandValidator : AbstractValidator<Land>
{
    public LandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(30);

        RuleFor(x => x.ISO2)
            .Length(2);

        RuleFor(x => x.ISO3)
            .Length(3);
    }
}
