using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class WährungValidator : AbstractValidator<Währung>
{
    public WährungValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Code)
            .MaximumLength(3);

        RuleFor(x => x.Nachkommastellen)
            .GreaterThanOrEqualTo(0);
    }
}
