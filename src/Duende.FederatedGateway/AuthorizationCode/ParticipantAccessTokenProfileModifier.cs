using System.Security.Claims;
using Duende.FederatedGateway.AuthorizationCode;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace AuthDuendeentication.FederatedGateway.AuthorizationCode;

public class ParticipantAccessTokenProfileModifier : IProfileModifier
{
    public Task ModifyProfile(ProfileDataRequestContext context)
    {
        var client = context.ValidatedRequest?.Client;

        if (context.Caller != IdentityServerConstants.ProfileDataCallers.ClaimsProviderAccessToken)
        {
            return Task.CompletedTask;
        }
        
        var subjectClaims = context.Subject.Claims.ToList();

      
        context.IssuedClaims = subjectClaims;

        return Task.CompletedTask;
    }
}
