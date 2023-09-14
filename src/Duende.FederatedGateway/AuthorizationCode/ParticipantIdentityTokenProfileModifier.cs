using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Duende.FederatedGateway.AuthorizationCode;

public class ParticipantIdentityTokenProfileModifier : IProfileModifier
{
    private const string ViaBenefitsSystemId = "viabenefits";

    public Task ModifyProfile(ProfileDataRequestContext context)
    {
        var client = context.ValidatedRequest?.Client;

        if (context.Caller != IdentityServerConstants.ProfileDataCallers.ClaimsProviderIdentityToken)
        {
            return Task.CompletedTask;
        }

        var idTokenClaims = new List<Claim>();

        var subjectClaims = context.Subject.Claims.ToList();

      
        context.IssuedClaims = subjectClaims;

        return Task.CompletedTask;
    }
}
