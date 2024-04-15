namespace BlazorERP.Core.Options;
public class LdapOptions
{
    public const string SectionName = "LdapOptions";
    public bool EnableLdapLogin { get; set; }
    public string LDAP_SERVER { get; set; } = string.Empty;
    public string DOMAIN_SERVER { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string GroupBaseOU { get; set; } = string.Empty;
}

