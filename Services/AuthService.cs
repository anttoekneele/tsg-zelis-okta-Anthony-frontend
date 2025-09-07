using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public class AuthService : IAuthService
{
    public async Task AuthLogin(TokenValidatedContext context)
    {
        var accessToken = context.TokenEndpointResponse?.AccessToken;
        var idToken = context.TokenEndpointResponse?.IdToken;
        var refreshToken = context.TokenEndpointResponse?.RefreshToken;

        Console.WriteLine($"Access Token: {accessToken}");
        Console.WriteLine($"ID Token: {idToken}");
        Console.WriteLine($"Refresh Token: {refreshToken}");

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

        /*
        Call user service to get role and navigate to roleclaims to get permissions.
        Add permissions as new claims here.
        */

        await Task.CompletedTask;
    }

    public Task AuthLogout()
    {
        throw new NotImplementedException();
    }
}