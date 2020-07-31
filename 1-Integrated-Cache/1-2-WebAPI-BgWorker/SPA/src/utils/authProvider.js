import React, { Component } from "react";
import { PublicClientApplication, InteractionRequiredAuthError } from "@azure/msal-browser";
import { msalConfig, loginRequest } from './authConfig';

export const isIE = () => {
    const ua = window.navigator.userAgent;
    const msie = ua.indexOf("MSIE ") > -1;
    const msie11 = ua.indexOf("Trident/") > -1;

    // If you as a developer are testing using Edge InPrivate mode, please add "isEdge" to the if check
    // const isEdge = ua.indexOf("Edge/") > -1;

    return msie || msie11;
};

// If you support IE, our recommendation is that you sign-in using Redirect flow
const useRedirectFlow = isIE();

const msalApp = new PublicClientApplication(msalConfig);

export default WrappedComponent => class AuthProvider extends Component {

    constructor(props) {
        super(props);

        this.state = {
            account: null,
            error: null,
            accessToken: null,
            username: null,
            isAuthenticated: false,
        };
    }

    componentDidMount = () => {
        if (useRedirectFlow) {
            msalApp.handleRedirectPromise()
                .then(this.handleResponse)
                .catch(err => {
                    this.setState({error: err.errorMessage});
                    console.error(err);
                });
        }
    }

    getAccounts = () => {
        /**
         * See here for more info on account retrieval: 
         * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-common/docs/Accounts.md
         */
        const currentAccounts = msalApp.getAllAccounts();
        
        if (currentAccounts === null) {
            return;
        } else if (currentAccounts.length > 1) {
            console.warn("Multiple accounts detected.");
            // Add choose account code here
            this.setState({username: currentAccounts[0].username})
        } else if (currentAccounts.length === 1) {
            this.setState({username: currentAccounts[0].username})
        }
    }

    handleResponse = (response) => {
        console.log(response);
        if (response !== null) {
            this.setState({
                account: response.account,
                accessToken: response.accessToken, 
                username: response.account.username,
                isAuthenticated: true,
            });
        } else {
            this.getAccounts();
        }
    }

    acquireToken = (request) => {
            /**
             * See here for more info on account retrieval: 
             * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-common/docs/Accounts.md
             */
            request.account = msalApp.getAccountByUsername(this.state.username);
            return msalApp.acquireTokenSilent(request).catch(error => {
                console.warn("silent token acquisition fails. acquiring token using redirect");
                if (error instanceof InteractionRequiredAuthError) {
                    // fallback to interaction when silent call fails
                    return msalApp.acquireTokenPopup(request)
                        .then(this.handleResponse)
                        .catch(error => {
                            console.error(error);
                        });
                } else {
                    console.warn(error);   
                }
            });
    }

    signIn = (redirect) => {
        if (redirect) {
            return msalApp.loginRedirect(loginRequest);
        }

        return msalApp.loginPopup(loginRequest)
            .then(this.handleResponse)
            .catch(err => {
                this.setState({err: err.errorMessage});
            });
    }

    signOut = () => {
        const logoutRequest = {
            account: msalApp.getAccountByUsername(this.state.username)
        };
    
        return msalApp.logout(logoutRequest);
    }

    render() {
        console.log(this.state)
        return (
            <WrappedComponent
                {...this.props}
                account = {this.state.account}
                error = {this.state.error}
                accessToken = {this.state.accessToken}
                isAuthenticated = {this.state.isAuthenticated}
                signIn = {() => this.signIn(useRedirectFlow)}
                signOut = {() => this.signOut()}
                acquireToken = {(request) => this.acquireToken(request)}
            />
        );
    }
};