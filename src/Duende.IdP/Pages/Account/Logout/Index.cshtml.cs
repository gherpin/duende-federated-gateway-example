using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.Saml.Services;

namespace Duende.IdP.Pages.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ISamlInteractionService _samlInteractionService;
    private readonly IEventService _events;

    [BindProperty] 
    public string LogoutId { get; set; }

    public Index(IIdentityServerInteractionService interaction, ISamlInteractionService samlInteractionService, IEventService events)
    {
        _interaction = interaction;
        _samlInteractionService = samlInteractionService;
        _events = events;
    }

    public async Task<IActionResult> OnGet(string logoutId, string requestId)
    {
      //For Idp Initiated Logout logoutId and requestId is null
      //For SP Initiated SLO, logoutIf and requestId is present
        LogoutId = logoutId;

        var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        if (User?.Identity.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            var context = await _interaction.GetLogoutContextAsync(LogoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showLogoutPrompt = false;
            }
        }
            
        if (showLogoutPrompt == false)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await OnPost(logoutId, requestId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(string logoutId, string requestId)
    {
        if (User?.Identity.IsAuthenticated == true)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed
            LogoutId ??= await _interaction.CreateLogoutContextAsync();
            var logoutContext = await _interaction.GetLogoutContextAsync(LogoutId);
                
            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            // raise the logout event
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // see if we need to trigger federated logout
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string url = Url.Page("/Account/Logout/Loggedout", new { logoutId = LogoutId });

                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }

        //Logout requests from Fed Gateway will have request Id
       
       //     var completionUrl = await _samlInteractionService.GetLogoutCompletionUrl(LogoutId);
           
       //    await _samlInteractionService.ExecuteIterativeSlo(HttpContext, LogoutId, completionUrl);
       //    return new EmptyResult(); //Browser url shows request to localhost:5004/saml/slo?SAMLRequest instead of going to Idp LoggedOut page
       
         
        }

        return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId, requestId = requestId });
    }
}