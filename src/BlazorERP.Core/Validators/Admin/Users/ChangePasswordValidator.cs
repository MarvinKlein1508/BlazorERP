using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Users;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.PasswordNew)
            .NotEmpty()
            .WithMessage("Bitte geben ein Passwort ein.");

        RuleFor(x => x.PasswordNew)
            .MinimumLength(8)
            .When(x => !string.IsNullOrEmpty(x.PasswordNew))
            .WithMessage("Ihr Passwort muss mindestens 8 Zeichen haben");

        RuleFor(x => x.PasswordConfirm)
            .Must((input, val) => input.PasswordNew.Equals(val))
            .WithMessage("Passwörter stimmen nicht überein");

        RuleFor(x => x.PasswordOld)
            .MinimumLength(8)
            .When(x => x.RequireOldPassword)
            .WithMessage("Bitte geben Sie Ihr altes Kennwort ein");
    }
}
