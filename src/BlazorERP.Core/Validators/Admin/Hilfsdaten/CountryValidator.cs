using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class CountryValidator : AbstractValidator<Country>
{
    public CountryValidator()
    {

        RuleFor(x => x.ISO2)
            .Length(2);

        RuleFor(x => x.ISO3)
            .Length(3);
    }
}
