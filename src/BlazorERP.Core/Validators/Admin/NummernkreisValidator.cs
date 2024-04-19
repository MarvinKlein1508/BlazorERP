using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin;

public class NummernkreisValidator : AbstractValidator<Nummernkreis>
{
    public NummernkreisValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.KundeBis)
            .Must((obj, val) => val > obj.KundeVon)
            .WithMessage("Kunde Bis muss größer sein als Kunde Von");


    }
}
