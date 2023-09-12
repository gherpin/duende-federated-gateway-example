# Federated Gateway Example

This example is adapted from Rock Solid Knowledge's sample repoistory found at
https://github.com/RockSolidKnowledge/Samples.Saml2p

Updated Each occurence of
```
options.Licensee =  "{Input Licensee}";
options.LicenseKey =  "{Input LicenseKey}";
```

with your information from Rock Solid Knowledge.



### To run from VS Code, Select Run <b>Federated Gateway Example</b>

## Duende.SP

Runs on https://localhost:5002

This application uses an OIDC connection to authenticate via the Duende.FederatedGateway Project
This application does not contain any saml endpoints.

## Duende.FederatedGateway

Runs on https://localhost:5004

A Federated Gateway that has been configured to provide SAML endpoints and obtain identity from 
Duende.IdP.

## Duende.IdP

Runs on https://localhost:5000

This application has been configured to act as a Saml Enabled Identity Provider, with the Duende.FederatedGateway configured to act as a service provider.