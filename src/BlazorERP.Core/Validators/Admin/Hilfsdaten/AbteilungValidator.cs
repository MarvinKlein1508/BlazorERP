using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class AbteilungValidator : AbstractValidator<Abteilung>
{
    public AbteilungValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.ActiveDirectoryGroupCN)
            .MaximumLength(255);
    }
}
