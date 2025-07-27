using System.Security.Claims;

namespace Shared.Application.Interfaces;

public interface IIdentityService
{
    IdentityEntity? GetIdentity(ClaimsPrincipal user);
    
    IdentityEntity GetCurrentUser();
}

public class IdentityEntity
{
    public string UserId { get; set; }
    
    public string Email { get; set; }
    
    public string FullName { get; set; }
    
    public string RoleName { get; set; }
}