using BlazorERP.Core.Extensions;
using BlazorERP.Core.Models;
using BlazorERP.Core.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;

namespace BlazorERP.Core.Utilities;
public class ActiveDirectoryManager
{
    private readonly ActiveDirectorySettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;

    public bool IsEnabled => _settings.IsEnabled;
    public ActiveDirectoryManager(IOptions<ActiveDirectorySettings> settings, UserManager<ApplicationUser> userManager)
    {
        _settings = settings.Value;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> LoginAsync(string username, string password)
    {

        if (!_settings.IsEnabled)
        {
            return null;
        }

        try
        {
            LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(_settings.DomainServer);
            using var connection = new LdapConnection(identifier);

            var networkCredential = new NetworkCredential(username, password, _settings.DomainServer);
            connection.SessionOptions.SecureSocketLayer = false;
            connection.AuthType = AuthType.Negotiate;
            connection.Bind(networkCredential);

            var searchRequest = new SearchRequest
            (
                distinguishedName: _settings.DistinguishedName,
                ldapFilter: $"(SAMAccountName={username})",
                searchScope: SearchScope.Subtree,
                attributeList:
                [
                    "cn",
                    "mail",
                    "displayName",
                    "givenName",
                    "sn",
                    "objectGUID",
                    "memberOf"
                ]
            );

            SearchResponse directoryResponse = (SearchResponse)connection.SendRequest(searchRequest);
            SearchResultEntry searchResultEntry = directoryResponse.Entries[0];

            Dictionary<string, string> attributes = [];
            Guid? guid = null;

            List<string> active_directory_user_groups = [];
            foreach (DirectoryAttribute userReturnAttribute in searchResultEntry.Attributes.Values)
            {
                if (userReturnAttribute.Name == "objectGUID")
                {
                    byte[] guidByteArray = (byte[])userReturnAttribute.GetValues(typeof(byte[]))[0];
                    guid = new Guid(guidByteArray);
                    attributes.Add("guid", ((Guid)guid).ToString());
                }
                else if (userReturnAttribute.Name == "memberOf")
                {
                    foreach (string item in userReturnAttribute.GetValues(typeof(string)).Cast<string>())
                    {
                        active_directory_user_groups.Add(item);
                    }
                }
                else
                {
                    attributes.Add(userReturnAttribute.Name, (string)userReturnAttribute.GetValues(typeof(string))[0]);
                }
            }

            attributes.TryAdd("mail", string.Empty);
            attributes.TryAdd("sn", string.Empty);
            attributes.TryAdd("givenName", string.Empty);
            attributes.TryAdd("displayName", string.Empty);

            if (guid is null)
            {
                throw new InvalidOperationException();
            }


            var user = await _userManager.GetByActiveDirectoryGuidAsync((Guid)guid);

            if (user is null)
            {
                // We need to create a new user in the database
                user = new ApplicationUser
                {
                    ActiveDirectoryGuid = guid,
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    PasswordHash = string.Empty,
                    SecurityStamp = string.Empty,
                    EmailConfirmed = true,
                    Email = attributes["mail"],
                    NormalizedEmail = attributes["mail"].ToUpper()
                };

                await _userManager.CreateAsync(user);
            }
            else
            {
                // We need to update the properties
                user.UserName = username;
                user.NormalizedUserName = username.ToUpper();
                user.Email = attributes["mail"];
                user.NormalizedEmail = user.Email.ToUpper();

                await _userManager.UpdateAsync(user);
            }

            return user;
        }
        catch (LdapException ex)
        {
           
        }
        
        return null;
    }
}
