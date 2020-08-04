// For a full list of msal.js configuration parameters, 
// visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html

export const msalConfig = {
    auth: {
        clientId: "Enter the Client Id (aka 'Application ID')",
        authority: "Enter the Authority, e.g 'https://login.microsoftonline.com/{tid}'",
        redirectUri: "http://localhost:3000",
    },
    cache: {
        cacheLocation: "localStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    },
}


export const webApiConfig = {
    apiURI: "Enter the WebAPI URI, e.g. 'https://localhost:44351/api/profile'",
    resourceScope: "Enter the API scopes as declared in the app registration 'Expose an Api' blade in the form of 'api://{client_id}/.default'"
}

/** 
 * Scopes you enter here will be consented once you authenticate. For a full list of available authentication parameters, 
 * visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html
 */
export const loginRequest = {
    scopes: ["openid", "profile", webApiConfig.resourceScope]
}

// Add here scopes for access token to be used at the API endpoint.
export const tokenRequest = {
    scopes: [webApiConfig.resourceScope, "offline_access"]
}
