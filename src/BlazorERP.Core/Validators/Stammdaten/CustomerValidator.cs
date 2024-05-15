using BlazorERP.Core.Models;
using FluentValidation;
using FluentValidation.Results;

namespace BlazorERP.Core.Validators.Stammdaten;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Company)
                .NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Name1));

        RuleFor(x => x.Company)
            .MaximumLength(60);

        RuleFor(x => x.Name1)
                .NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Company));

        RuleFor(x => x.Name1)
            .MaximumLength(60);

        RuleFor(x => x.Name2)
            .MaximumLength(60);

        RuleFor(x => x.Street)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(x => x.City)
            .MaximumLength(40)
            .NotEmpty();

        RuleFor(x => x.CountryId)
            .NotNull()
            .WithMessage("Wählen Sie ein Land aus.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(8)
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotNull()
            .WithMessage("Wählen Sie eine Sprache aus.");

        RuleFor(x => x.CurrencyCode)
            .NotNull()
            .WithMessage("Wählen Sie eine Währung aus.");

        RuleFor(x => x.SalutationId)
            .NotNull()
            .WithMessage("Wählen Sie eine Anrede aus.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30);

        RuleFor(x => x.MobileNumber)
            .MaximumLength(30);

        RuleFor(x => x.FaxNumber)
            .MaximumLength(30);

       

        RuleFor(x => x.Email)
            .MaximumLength(255)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);

        RuleFor(x => x.Website)
            .MaximumLength(60);

      
        RuleFor(x => x.PaymentConditionId)
            .NotNull()
            .WithMessage("Wählen Sie eine Zahlungsbedingung aus.");

        RuleFor(x => x.DeliveryConditionId)
            .NotNull()
            .WithMessage("Wählen Sie eine Lieferbedingung aus.");


        RuleFor(kund => kund.CreditLimit)
            .GreaterThanOrEqualTo(0);

        RuleFor(kund => kund.VatIdentificationNumber)
            .MaximumLength(30);

        RuleFor(x => x.IBAN)
            .MaximumLength(40);

        RuleFor(x => x.BIC)
            .MaximumLength(15);

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
