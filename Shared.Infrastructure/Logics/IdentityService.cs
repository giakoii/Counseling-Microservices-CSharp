using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Shared.Application.Interfaces;

namespace Shared.Infrastructure.Logics;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IdentityEntity? GetIdentity(ClaimsPrincipal user)
    {
        var identity = user.Identity as ClaimsIdentity;
        if (!identity.IsAuthenticated)
            return null;
        
        // Get id
        var id = identity.FindFirst(OpenIddictConstants.Claims.Subject)!.Value;
        
        // Get email
        var email = identity!.FindFirst(OpenIddictConstants.Claims.Email)!.Value;
        
        // Get name
        var name = identity.FindFirst(OpenIddictConstants.Claims.Name)?.Value;
        
        // Get role
        var role = identity.FindFirst(OpenIddictConstants.Claims.Role)?.Value;
        
        // Create IdentityEntity
        var identityEntity = new IdentityEntity
        {
            UserId = id,
            Email = email,
            FullName = name!,
            RoleName = role!,
        };
        return identityEntity;
    }

    public IdentityEntity GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return GetIdentity(user);
    }
}