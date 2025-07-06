using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Common.SystemClient;

public class IdentityApiClient : IIdentityApiClient
{
    public IdentityEntity GetIdentity(ClaimsPrincipal user)
    {
        var identity = user.Identity as ClaimsIdentity;
        
        // Get id
        var id = identity?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        
        // Get email
        var email = identity!.FindFirst(OpenIddictConstants.Claims.Email)?.Value;
        
        // Get phone number
        var phoneNumber = identity.FindFirst(OpenIddictConstants.Claims.PhoneNumber)?.Value;
        
        // Get address
        var address = identity.FindFirst(OpenIddictConstants.Claims.Address)?.Value;
        
        // Get role name
        var roleName = identity.FindFirst(OpenIddictConstants.Claims.Role)?.Value;
        
        // Create IdentityEntity
        var identityEntity = new IdentityEntity
        {
            UserId = id!,
            Email = email!,
            PhoneNumber = phoneNumber!,
            Address = address!,
            RoleName = roleName!,
        };
        return identityEntity;
    }
}