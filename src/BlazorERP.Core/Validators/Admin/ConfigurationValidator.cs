using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin;

public class ConfigurationValidator : AbstractValidator<Configuration>
{
    public ConfigurationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.CustomerCreditLimit)
            .GreaterThanOrEqualTo(0);

    }
}
