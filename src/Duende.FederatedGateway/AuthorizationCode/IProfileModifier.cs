using Duende.IdentityServer.Models;

namespace Duende.FederatedGateway.AuthorizationCode;

public interface IProfileModifier
{
    Task ModifyProfile(ProfileDataRequestContext context);
}
