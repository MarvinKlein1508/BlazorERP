using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators;

public class TranslationValidator : AbstractValidator<Translation>
{
    public TranslationValidator()
    {
        RuleFor(x => x.ValueText)
            .NotEmpty();

        RuleFor(x => x.ValueText)
            .MaximumLength(255);
    }
}
