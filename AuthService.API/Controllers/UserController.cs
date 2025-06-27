using System.Security.Claims;
using AuthService.API.Helpers;
using AuthService.Application.Users.Commands;
using Common.SystemClient;
using Common.Utils.Const;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IMediator _mediator;
    private readonly IIdentityApiClient _identityApiClient;

    public UserController(IMediator mediator, IOpenIddictScopeManager scopeManager, IIdentityApiClient identityApiClient)
    {
        _mediator = mediator;
        _scopeManager = scopeManager;
        _identityApiClient = identityApiClient;
    }
    
    [HttpPost]
    [Route("[action]")]
    public UserCreateResponse CreateUser([FromBody] InsertUserCommand request)
    {
        try
        {
            var response = _mediator.Send(request).Result;
            return response;
        }
        catch (Exception e)
        {
            return new UserCreateResponse
            {
                Success = false,
                MessageId = MessageId.E00000,
                Message = e.Message
            };
        }
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public SelectUserProfileResponse SelectUserProfile([FromQuery] SelectUserProfileQuery request)
    {
        var identity = _identityApiClient.GetIdentity(User);
        
        if (identity == null)
        {
            var response = new SelectUserProfileResponse { Success = false };
            response.SetMessage(MessageId.E11001);
            return response;
        }
        
        var result = _mediator.Send(request with {UserId = Guid.Parse(identity.UserId)}).Result;
        return result;
    }
    
    /// <summary>
    /// Exchange token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange([FromForm]LoginUserCommand request)
    {
        var openIdRequest = HttpContext.GetOpenIddictServerRequest();

        // Password
        if (openIdRequest!.IsPasswordGrantType())
        {
            return await TokensForPasswordGrantType(request);
        }

        // Refresh token
        if (openIdRequest!.IsRefreshTokenGrantType())
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            return SignIn(claimsPrincipal!, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Unsupported grant type
        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType
        });
    }
    
    /// <summary>
    /// Generate tokens for the user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> TokensForPasswordGrantType(LoginUserCommand request)
    {
        // Else user use password login
        var userPasswordLogin = await _mediator.Send(request);
        if (!userPasswordLogin.Success)
        {
            return Unauthorized(new OpenIddictResponse
            {
                Error = Errors.InvalidRequest,
                ErrorDescription = userPasswordLogin.Message,
            });
        }

        // Create claims
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role
        );

        // Set claims
        identity.SetClaim(Claims.Subject, userPasswordLogin.Response.UserId.ToString(),
            Destinations.AccessToken);
        identity.SetClaim(Claims.Name, userPasswordLogin.Response.FullName,
            Destinations.AccessToken);
        identity.SetClaim("UserId", userPasswordLogin.Response.UserId.ToString(),
            Destinations.AccessToken);
        identity.SetClaim(Claims.Email, userPasswordLogin.Response.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.PhoneNumber, userPasswordLogin.Response.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Role, userPasswordLogin.Response.RoleName,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Audience, "service_client",
            Destinations.AccessToken);

        identity.SetDestinations(claim =>
        {
            return claim.Type switch
            {
                Claims.Subject => new[] { Destinations.AccessToken },
                Claims.Name => new[] { Destinations.AccessToken },
                "UserId" => new[] { Destinations.AccessToken },
                Claims.Email => new[] { Destinations.AccessToken },
                Claims.Role => new[] { Destinations.AccessToken },
                Claims.Audience => new[] { Destinations.AccessToken },
                _ => new[] { Destinations.AccessToken }
            };
        });

        // Set scopes
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(new string[]
        {
            Scopes.Roles,
            Scopes.OfflineAccess,
            Scopes.Profile,
        });

        claimsPrincipal.SetResources(await _scopeManager.ListResourcesAsync(claimsPrincipal.GetScopes()).ToListAsync());

        // Set refresh token and access token
        claimsPrincipal.SetAccessTokenLifetime(TimeSpan.FromHours(1));
        claimsPrincipal.SetRefreshTokenLifetime(TimeSpan.FromHours(2));

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
