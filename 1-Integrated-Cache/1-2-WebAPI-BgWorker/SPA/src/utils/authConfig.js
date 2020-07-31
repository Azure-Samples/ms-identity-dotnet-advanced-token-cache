// For a full list of msal.js configuration parameters, 
// visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html

export const msalConfig = {
    auth: {
        clientId: "280b56c5-087e-4544-9f0d-95967d9fc1fe",
        authority: "https://login.microsoftonline.com/da0cc090-dd32-4d9f-984c-130b337f221c",
        redirectUri: "http://localhost:3000",
    },
    cache: {
        cacheLocation: "localStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    },
}


export const webApiConfig = {
    apiURI: "https://localhost:44351/api/profile",
    resourceScope: "api://292e9306-4fba-44f4-bd28-748f02934c1b/.default"
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
