using System.Security.Claims;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Duende.FederatedGateway.Controllers;

[AllowAnonymous]
[Route("[controller]/[action]")]
public class BrokerController : Controller {

     [HttpGet]
    public async Task<IActionResult> SamlCallback(CancellationToken cancellationToken)
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (!result.Succeeded)
        {
            Log.Error(result.Failure!, "External authentication error");
            throw new Exception("External authentication error");
        }

        var externalClaims = result.Principal?.Claims.ToList(); // ?? Enumerable.Empty<Claim>().ToList();
        if (!result.Properties.Items.TryGetValue("scheme", out var scheme))
        {
            const string msg = "Unable to retrieve scheme {@Data}";
            Log.Error(msg, result.Properties.Items);
            throw new Exception(msg);
        }

        var isUser = new IdentityServerUser(result.Principal.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            IdentityProvider = result.Properties?.Items["scheme"],
            AdditionalClaims = externalClaims
        };

        var localSignInProps = new AuthenticationProperties();
        await HttpContext.SignInAsync(isUser, localSignInProps);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties?.Items["returnUrl"] ?? "~/";

        Log.Information("Redirect to {RedirectUrl}", returnUrl);
        return Redirect(returnUrl);
    }
}