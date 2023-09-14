using System.Security.Cryptography.X509Certificates;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using Rsk.AspNetCore.Authentication.Saml2p;
using Rsk.Saml.DuendeIdentityServer.DynamicProviders;
using Serilog;

namespace Duende.FederatedGateway;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        //builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            });
           // .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
        isBuilder.AddInMemoryClients(Config.Clients);

        // SP configuration - dynamic providers
        isBuilder.AddSamlDynamicProvider(options =>
        {
            // unstorable/reusable data, such as license information and events. This will override the data stored
            options.Licensee =  "{Input Licensee}";
            options.LicenseKey =  "{Input LicenseKey}";
            options.SignedOutCallbackPath = "/federation/saml/slo";  //Takes a SAMLRESPONSE during SP initiated SLO
            options.LogSamlMessages = true;
            options.TimeComparisonTolerance = 300;
        })
           
            // Use EntityFramework store for storing identity providers
            //.AddIdentityProviderStore<SamlIdentityProviderStore>();

            // use in memory store for storing identity providers
            .AddInMemoryIdentityProviders(new List<IdentityProvider>
            {
                    new SamlDynamicIdentityProvider
                    {   
                        
                        Scheme = "saml",
                        DisplayName = "saml",
                        Enabled = true,
                        SamlAuthenticationOptions = new Saml2pAuthenticationOptions
                        {
                            CallbackPath = "/federation/saml/signin-saml", // Duende prefixes "/federation/{scheme}/{suffix}" to all paths
                            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                            SignOutScheme = "idsrv", // main cookie user is signed into
                            TimeComparisonTolerance = 7200,
                            // The IdP you want to integrate with
                            IdentityProviderOptions = new IdpOptions
                            {
                                EntityId = "https://localhost:5000",
                                SigningCertificates = { new X509Certificate2("./src/Duende.FederatedGateway/idsrv3test.cer") },
                                SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                                SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect)
                            },

                            // Details about yourself (the SP) - In This care the Federated Gateway
                            ServiceProviderOptions = new SpOptions
                            {
                                EntityId = "https://localhost:5004/saml",
                                MetadataPath = "/federation/saml/metadata",
                                SignAuthenticationRequests = false // OPTIONAL - use if you want to sign your auth requests
                            }
                        }
                    }
            })
        ;

        
        builder.Services.AddAuthentication()
            //Register Federated Gateway as a SAML Service Provider
            .AddSaml2p("saml",options=>
            {
                options.Licensee =  "{Input Licensee}";;
                options.LicenseKey =  "{Input LicenseKey}";;
                
                options.IdentityProviderMetadataAddress = "https://localhost:5000/saml/metadata";

                options.CallbackPath = "/saml/sso";
                
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;

                
                
                options.ServiceProviderOptions = new SpOptions
                {
                    EntityId = "https://localhost:5004/saml",
                    MetadataPath = "/saml/metadata",
                    SignAuthenticationRequests = false,
                };

            
            });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
        app.UseDeveloperExceptionPage();
        app.UseIdentityServer();
        app.MapControllers();
        return app;
    }
}