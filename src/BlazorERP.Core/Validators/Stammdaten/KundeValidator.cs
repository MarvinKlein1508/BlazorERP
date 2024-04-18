using BlazorERP.Core.Models;
using FluentValidation;
using FluentValidation.Results;

namespace BlazorERP.Core.Validators.Stammdaten;

public class KundeValidator : AbstractValidator<Kunde>
{
    public KundeValidator()
    {
        RuleFor(x => x.Firma)
                .NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Name1));

        RuleFor(x => x.Firma)
            .MaximumLength(60);

        RuleFor(x => x.Name1)
                .NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Firma));

        RuleFor(x => x.Name1)
            .MaximumLength(60);

        RuleFor(x => x.Name2)
            .MaximumLength(60);

        RuleFor(x => x.Strasse)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(x => x.Ort)
            .MaximumLength(40)
            .NotEmpty();

        RuleFor(x => x.LandId)
            .NotNull()
            .WithMessage("Wählen Sie ein Land aus.");

        RuleFor(x => x.Postleitzahl)
            .MaximumLength(8)
            .NotEmpty();

        RuleFor(x => x.SprachId)
            .NotNull()
            .WithMessage("Wählen Sie eine Sprache aus.");

        RuleFor(x => x.Waehrungscode)
            .NotNull()
            .WithMessage("Wählen Sie eine Währung aus.");

        RuleFor(x => x.AnredeId)
            .NotNull()
            .WithMessage("Wählen Sie eine Anrede aus.");

        RuleFor(x => x.Telefonnummer)
            .MaximumLength(30);

        RuleFor(x => x.Mobilnummer)
            .MaximumLength(30);

        RuleFor(x => x.Faxnummer)
            .MaximumLength(30);

       

        RuleFor(x => x.Email)
            .MaximumLength(255)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);

        RuleFor(x => x.Website)
            .MaximumLength(60);

      

        RuleFor(x => x.LieferbedingungId)
            .NotNull()
            .WithMessage("Wählen Sie eine Lieferbedingung aus.");

        RuleFor(x => x.ZahlungsbedingungId)
            .NotNull()
            .WithMessage("Wählen Sie eine Zahlungsbedingung aus.");

       
        RuleFor(kund => kund.Kreditlimit)
            .GreaterThanOrEqualTo(0);

        RuleFor(kund => kund.UmsatzsteuerIdentifikationsnummer)
            .MaximumLength(30);

        RuleFor(x => x.IBAN)
            .MaximumLength(40);

        RuleFor(x => x.BIC)
            .MaximumLength(15);

        RuleFor(x => x.Notiz)
            .MaximumLength(1000);
    }
}
