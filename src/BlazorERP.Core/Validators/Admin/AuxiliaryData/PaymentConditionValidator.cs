using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class PaymentConditionValidator : AbstractValidator<PaymentCondition>
{
    public PaymentConditionValidator()
    {
     

        RuleFor(x => x.NetDays)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Discount1Days)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Discount2Days)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Discount1Percent)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Discount2Percent)
            .GreaterThanOrEqualTo(0);

    }
}
