using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin;

public class NumberRangeValidator : AbstractValidator<NumberRange>
{
    public NumberRangeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.CustomerTo)
            .Must((obj, val) => val > obj.CustomerFrom)
            .WithMessage("Kunde Bis muss größer sein als Kunde Von");


    }
}
