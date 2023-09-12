using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Rsk.Saml.Models;

namespace Duende.SP.Controllers;

public class LogoutController : Controller {


    /// <summary>
    /// This endpoint is used when SAML IdP calls the SP directly.
    /// </summary>
    /// <param name="samlRequest"></param>
    /// <param name="sampResponse"></param>
    /// <param name="relayState"></param>
    /// <param name="sigAlg"></param>
    /// <param name="signature"></param>
    /// <param name="connection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/saml/slo")]
    public async Task<IActionResult> FedAuthSsoLogout(string samlRequest, string samlResponse, string relayState, string sigAlg, string signature, string connection) {

        //TODO: Add Local Signout of cookie
        // await HttpContext.SignOoutAsync("cookiemonster); Only reason call is made from IdP to SP

        var uriBuilder = new UriBuilder($"https://localhost:5004/") {
            Path = $"/federation/saml/slo"
        };

        //Create FederatedGateway Request
        var queryParameters = new Dictionary<string, string>{
            {"RelayState", relayState},
            {"SigAlg", sigAlg},
            {"Signature", signature}
        };

        if (!string.IsNullOrWhiteSpace(samlRequest)) {
            queryParameters.Add("SamlRequest", samlRequest);
        }

        if (!string.IsNullOrWhiteSpace(samlResponse)){
            queryParameters.Add("SAMLResponse", samlResponse);
        }

        var fedGatewayLogoutUri = QueryHelpers.AddQueryString(uriBuilder.Uri.ToString(), queryParameters);
        return Redirect(fedGatewayLogoutUri);
    }
}