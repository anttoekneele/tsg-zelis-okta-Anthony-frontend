using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using tsg_zelis_okta_Anthony_frontend.Components;

var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultScheme = "Cookies";
      options.DefaultChallengeScheme = "Okta"; // default button can be Okta
  })
  .AddCookie("Cookies", o => { o.SlidingExpiration = true; })
  .AddOpenIdConnect("Okta", o =>
  {
      o.Authority = builder.Configuration["Okta:Authority"]; // e.g., https://dev-xxx.okta.com/oauth2/default
      o.ClientId = builder.Configuration["Okta:ClientId"];
      o.ClientSecret = builder.Configuration["Okta:ClientSecret"];
      o.ResponseType = "code";
      o.UsePkce = true;
      o.SaveTokens = true;
      o.Scope.Clear();
      o.Scope.Add("openid"); o.Scope.Add("profile"); o.Scope.Add("email");
      o.GetClaimsFromUserInfoEndpoint = true;
      // Hook events for audit logging:
      o.Events = new OpenIdConnectEvents {
        OnTokenValidated = AuditLoginAsync
      };
  });
//   .AddOpenIdConnect("Google", o =>
//   {
//       o.Authority = "https://accounts.google.com";
//       o.ClientId = builder.Configuration["Google:ClientId"];
//       o.ClientSecret = builder.Configuration["Google:ClientSecret"];
//       o.ResponseType = "code";
//       o.UsePkce = true;
//       o.SaveTokens = true;
//       o.Scope.Clear();
//       o.Scope.Add("openid"); o.Scope.Add("profile"); o.Scope.Add("email");
//       o.GetClaimsFromUserInfoEndpoint = true;
//       o.Events = new OpenIdConnectEvents {
//         OnTokenValidated = AuditLoginAsync
//       };
//   });

async Task AuditLoginAsync(TokenValidatedContext context)
{
    var identity = (ClaimsIdentity)context.Principal.Identity!;
    var nameClaim = identity.FindFirst("name") ?? identity.FindFirst("email");

    if (nameClaim != null)
    {
        identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Name, nameClaim.Value));
    }

    await Task.CompletedTask;
}

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewAuthEvents",
        p => p.RequireClaim("permissions", "Audit.ViewAuthEvents"));
    options.AddPolicy("CanViewRoleChanges",
        p => p.RequireClaim("permissions", "Audit.ViewRoleChanges"));
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/signin/okta", async context =>
{
    await context.ChallengeAsync("Okta", new AuthenticationProperties
    {
        RedirectUri = "/" // or "/dashboard", or whatever landing page
    });
});

app.MapGet("/signin/google", async context =>
{
    await context.ChallengeAsync("Google", new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});

app.MapGet("/signout", async context =>
{
    await context.SignOutAsync("Cookies", new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
