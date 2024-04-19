using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin;

public class VoreinstellungValidator : AbstractValidator<Voreinstellung>
{
    public VoreinstellungValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.KundeKreditlimit)
            .GreaterThanOrEqualTo(0);

    }
}
