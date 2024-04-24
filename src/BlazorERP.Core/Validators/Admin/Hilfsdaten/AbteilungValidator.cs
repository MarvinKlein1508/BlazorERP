using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class AbteilungValidator : AbstractValidator<Department>
{
    public AbteilungValidator()
    {



        RuleFor(x => x.ActiveDirectoryGroupCN)
            .MaximumLength(255);
    }
}
