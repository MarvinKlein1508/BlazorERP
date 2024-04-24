using BlazorERP.Core.Models;
using FluentValidation;

namespace BlazorERP.Core.Validators.Admin.Hilfsdaten;

public class DepartmentValidator : AbstractValidator<Department>
{
    public DepartmentValidator()
    {



        RuleFor(x => x.ActiveDirectoryGroupCN)
            .MaximumLength(255);
    }
}
