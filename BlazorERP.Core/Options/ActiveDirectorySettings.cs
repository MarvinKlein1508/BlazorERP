namespace BlazorERP.Core.Options;
public class ActiveDirectorySettings
{
    public const string SectionName = "ActiveDirectorySettings";
    public bool IsEnabled { get; set; }
    public string DomainServer { get; set; } = string.Empty;
    public string DomainName { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
}

