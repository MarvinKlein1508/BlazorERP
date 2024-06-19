using BlazorERP.Application.Models;
using BlazorERP.Contracts.Responses;

namespace BlazorERP.Api.Mappings;

public static class UserContractMapping
{
    public static UserResponse MapToResponse(this User item)
    {
        return new UserResponse
        {
            ActiveDirectoryGuid = item.ActiveDirectoryGuid,
            CreatedAt = item.CreatedAt,
            CreatedBy = item.CreatedBy,
            Email = item.Email,
            Firstname = item.Firstname,
            IsActive = item.IsActive,
            IsAdmin = item.IsAdmin,
            LastModified = item.LastModified,
            LastModifiedBy = item.LastModifiedBy,
            Lastname = item.Lastname,
            UserId = item.UserId,   
            Username = item.Username,
        };
    }
}
