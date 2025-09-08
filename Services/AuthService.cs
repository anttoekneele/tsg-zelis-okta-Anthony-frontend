using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private string testEmail = ""; // NEED TO REMOVE!

    public AuthService(IUserService userService, IAuditService auditService)
    {
        _userService = userService;
        _auditService = auditService;
    }

    public async Task OktaAuthLogin(TokenValidatedContext context)
    {
        var accessToken = context.TokenEndpointResponse?.AccessToken;
        var idToken = context.TokenEndpointResponse?.IdToken;
        var refreshToken = context.TokenEndpointResponse?.RefreshToken;

        if (!string.IsNullOrEmpty(accessToken))
        {
            var claimsIdentity = (ClaimsIdentity)context.Principal?.Identity!;
            claimsIdentity?.AddClaim(new Claim("access_token", accessToken));
        }

        if (!string.IsNullOrEmpty(idToken))
        {
            var claimsIdentity = (ClaimsIdentity)context.Principal?.Identity!;
            claimsIdentity?.AddClaim(new Claim("id_token", idToken));
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var claimsIdentity = (ClaimsIdentity)context.Principal?.Identity!;
            claimsIdentity?.AddClaim(new Claim("refresh_token", refreshToken));
        }
        
        var identity = (ClaimsIdentity)context.Principal!.Identity!;

        var externalId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var email = identity.FindFirst("preferred_username")?.Value! + testEmail;

        var user = (await _userService.GetUsers())
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            await _userService.CreateUser(externalId, email);
        }

        await _auditService.LoginSuccessEvent(email, "Okta");



        // NEED TO REMOVE
        Console.WriteLine($"Access Token: {accessToken}");
        Console.WriteLine($"ID Token: {idToken}");
        Console.WriteLine($"Refresh Token: {refreshToken}");
    }

    public async Task AuthLogout(RedirectContext context)
    {
        var identity = context.HttpContext.User;
        var email = identity.FindFirst("preferred_username")?.Value! + testEmail;

        await _auditService.LogoutEvent(email);
    }
}