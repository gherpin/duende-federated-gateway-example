using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookiemonster";
        //options.DefaultChallengeScheme = "saml";
        options.DefaultChallengeScheme = "FedAuth";
    })
    .AddCookie("cookiemonster")
    .AddOpenIdConnect(
        "FedAuth",
        options => {
    
        options.Authority = $"https://localhost:5004/";
        options.CorrelationCookie.Name = "AMFedAuthCorrelation";
        options.ClientId = "Duende.SP";
        options.ClientSecret = "faa32bb9-ef61-4821-b503-f5d0dac35f3a";
        options.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(30);

        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.NonceCookie.HttpOnly = true;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.CallbackPath = new PathString("/signin-oidc");

        // Set response type to code
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = "form_post";

        // Configure the scope
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");
        options.Scope.Add("address");

        options.ClaimsIssuer = "FedAuth";
        options.UseTokenLifetime = false;
        options.DisableTelemetry = true;
        options.SaveTokens = true;

        options.SignedOutCallbackPath = new PathString("/signout-callback-oidc"); //This is post_logout_redirect_uri

        options.Events = new OpenIdConnectEvents {

             OnTokenValidated = async context => {

                 Console.WriteLine("Token Validated");
             },

             OnTicketReceived = async context => {

                Console.WriteLine("Ticket Received");
             }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseAuthentication();


app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(c =>
{
    c.MapDefaultControllerRoute();
});
    


app.Run();