using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Users;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Username)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .MaximumLength(50);

        RuleFor(x=> x.Email)
            .MaximumLength(255);

        RuleFor(x=> x.Password)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(x => x.PasswordConfirm)
            .Must((input, val) => input.Password.Equals(val))
            .WithMessage("Die Passwörter stimmen nicht überein.")
            .When(x => x.GetIdentifier() == null);
    }
}
