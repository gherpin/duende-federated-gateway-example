using Duende.FederatedGateway.AuthorizationCode;
using Duende.IdentityServer.Validation;
using IdentityModel;

namespace Duende.FederatedGateway.Validation;

//Called after a user has signed into the Idp and the /token endpoint is being called
public class TokenRequestValidator : ICustomTokenRequestValidator
{
    private readonly IEnumerable<IAuthorizationCodeTokenModifier> _authorizationCodeTokenModifiers;

    public TokenRequestValidator(
        IEnumerable<IAuthorizationCodeTokenModifier> authorizationCodeTokenModifiers
        )
    {
        _authorizationCodeTokenModifiers = authorizationCodeTokenModifiers;
    }

    public Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
        switch (context.Result.ValidatedRequest.GrantType)
        {
            case OidcConstants.GrantTypes.AuthorizationCode:
            {
                _authorizationCodeTokenModifiers.ToList().ForEach(m => m.ModifyContext(context));
                break;
            }
        }

        return Task.CompletedTask;
    }
}
