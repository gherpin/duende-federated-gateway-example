using System.Security.Claims;
using Duende.IdentityServer.Validation;
using Microsoft.Extensions.Options;

namespace Duende.FederatedGateway.AuthorizationCode;

public interface IAuthorizationCodeTokenModifier
{
    Task ModifyContext(CustomTokenRequestValidationContext context);
}

public class ParticipantAuthorizationCodeTokenModifier : IAuthorizationCodeTokenModifier
{  
    public ParticipantAuthorizationCodeTokenModifier()
    {
    }

    public Task ModifyContext(CustomTokenRequestValidationContext context)
    {
        var isError = VerifyRequiredClaimsByIdp(context);

        if (!isError)
        {
            
        }

        return Task.CompletedTask;
    }

    private bool VerifyRequiredClaimsByIdp(CustomTokenRequestValidationContext context)
    {
        var subjectClaims = context.Result.ValidatedRequest.Subject.Claims;
        
        return context.Result.IsError;
    }
}
