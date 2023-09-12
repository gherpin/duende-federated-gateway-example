using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duende.SP.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public ViewResult Index()
    {
        return View();
    }

    [Route("/sign-in")]
    [Authorize]
    public IActionResult SignIn()
    {
        return Redirect("/");
    }

    [Route("/sign-out")]
    public async Task FullLogout()
    {
        // Sign out of the application session ( cookie )
        // Sign out of the saml scheme, this will cause a redirect to SAML IDP to sign out
        //This is also known as SP-Initiated Sign Out
        //return SignOut( "cookie", "saml");

        await HttpContext.SignOutAsync("cookiemonster");
            // TODO: Figure out how we can determine which connection is being used so that we can sign out with either scheme
        await HttpContext.SignOutAsync("FedAuth");
    }
    
    [Route("/app-sign-out")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("cookiemonster");
        return Redirect("/");
    }
}