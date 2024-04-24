using BlazorERP.Core.Models.Abstract;
using BlazorERP.Core.Utilities;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Abstract;

public class AbstractTranslationValidator<TModel> : AbstractValidator<TModel> where TModel : TranslationBase
{
    public AbstractTranslationValidator()
    {
        RuleFor(x => x.Translations)
            .Must(x => x.Any(x => x.LanguageId == Storage.DEFAULT_LANGUAGE))
            .WithMessage("Bitte fügen Sie einen Namen für die Default Sprache hinzu.");
    }
}
