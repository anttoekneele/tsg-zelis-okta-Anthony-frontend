using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public interface IAuthService
{
    Task AuthLogin(TokenValidatedContext context);
    Task AuthLogout();
}