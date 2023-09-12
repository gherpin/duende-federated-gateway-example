using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Serilog;

namespace Duende.FederatedGateway.Controllers;

//This would be like routing controller
[AllowAnonymous]
[Route("[controller]/[action]")]
public class AccountController : Controller {

    private readonly IIdentityServerInteractionService _interaction;
     private readonly IAuthenticationSchemeProvider _schemeProvider;
    public AccountController( IIdentityServerInteractionService interaction, AuthenticationSchemeProvider schemeProvider)
    {
        _interaction = interaction;
        _schemeProvider = schemeProvider;
    }


     [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        var authSchemeName = "saml"; // context?.IdP; use saml scheme and no OIDC Duende.IdP scheme
        // if (context == null || authSchemeName == null || await _schemeProvider.GetSchemeAsync(authSchemeName) == null)
        // {
        //     const string noCxn = "No connection found";
        //     Log.Warning("Connection {AuthSchemeName} not found", authSchemeName);
        //     return NotFound(noCxn);
        // }

        var redirectUri = Url.Action(nameof(BrokerController.SamlCallback), "Broker");

        var props = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", authSchemeName }, //Hard Code all requests to go to Duende.IdP Project
            }
        };

        // if there is a "participant" in the query, then this is the Agent Access flow
        // var participantToken = context.Parameters.Get("participant");
        // if (!string.IsNullOrWhiteSpace(participantToken))
        // {
        //     props.Items.Add("participant", participantToken);
        // }

        Log.Information("Challenge {AuthSchemeName}", authSchemeName);
        return Challenge(props, authSchemeName);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId, string connection = "saml") {

        var authenticationProperties = new AuthenticationProperties() {
            RedirectUri = "https://localhost:5002/"
        };
        
        logoutId ??= await _interaction.CreateLogoutContextAsync();

        if (User.Identity.IsAuthenticated) {
             await HttpContext.SignOutAsync();
             await HttpContext.SignOutAsync(connection, authenticationProperties); //Signs out of IdP (SAML)
        }
       
        return new EmptyResult();

        //  return SignOut(
        //     authenticationProperties,
        //     IdentityServerConstants.DefaultCookieAuthenticationScheme,
        //     connection
        // );
    }



    
}