﻿using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using Rsk.Saml;
using Rsk.Saml.Models;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;
using IdentityModel;

namespace Duende.IdP;

public static class Config
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Address()
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new ApiResource[]
        {
            new ApiResource("api1", "My API #1")
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
            new Client
            {
                ClientId = "https://localhost:5004/saml",
                ClientName = "Duende Federated Gateway",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                AllowedScopes = {"openid", "profile", "email", "address"},
                PostLogoutRedirectUris = { 
                    "https://localhost:5002/signout-callback-oidc",
                    "https://localhost:5002/"
                }

            }
        };
    }

    public static IEnumerable<ServiceProvider> GetServiceProviders()
    {
        return new[]
        {
            new ServiceProvider
            {
                EntityId = "https://localhost:5004/saml",
                AssertionConsumerServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpPost , "https://localhost:5004/saml/sso"),
                    new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5004/signin-saml-3")
                },
                SingleLogoutServices =
                {   
                    new Service(SamlConstants.BindingTypes.HttpRedirect , "https://localhost:5004/federation/saml/slo")
                },
                ClaimsMapping = new Dictionary<string, string> {
                    // {"name", JwtClaimTypes.Name},
                }
            }
        };
    }
}
