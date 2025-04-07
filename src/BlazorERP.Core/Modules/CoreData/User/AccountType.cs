using System.ComponentModel.DataAnnotations;
using Localization = BlazorERP.Core.Ressources.Modules.CoreData.User;

namespace BlazorERP.Core.Modules.CoreData;
public enum AccountType
{
    [Display(Name = "LocalAccount", ResourceType = typeof(Localization.AccountType))]
    LocalAccount,
    [Display(Name = "ActiveDirectory", ResourceType = typeof(Localization.AccountType))]
    ActiveDirectory
}
