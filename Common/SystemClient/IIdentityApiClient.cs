using System.Security.Claims;

namespace Common.SystemClient;

public interface IIdentityApiClient
{
    public IdentityEntity? GetIdentity(ClaimsPrincipal user);
}