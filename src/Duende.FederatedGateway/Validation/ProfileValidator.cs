using Duende.FederatedGateway.AuthorizationCode;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace Duende.FederatedGateway.Validation;

public class ProfileValidator : IProfileService
{
    private readonly IEnumerable<IProfileModifier> _profileModifiers;

    public ProfileValidator(IEnumerable<IProfileModifier> profileModifiers)
    {
        _profileModifiers = profileModifiers;
    }

    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        _profileModifiers.ToList().ForEach(x => x.ModifyProfile(context));
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
