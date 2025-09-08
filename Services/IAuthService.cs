using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public interface IAuthService
{
    Task OktaAuthLogin(TokenValidatedContext context);

    // Task GoogleAuthLogin(TokenValidatedContext context);

    // Task MicrosoftAuthLogin(TokenValidatedContext context);

    Task AuthLogout(RedirectContext context);
}