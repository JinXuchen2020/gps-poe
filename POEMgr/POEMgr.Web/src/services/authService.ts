import {
  Configuration,
  PublicClientApplication,
  InteractionRequiredAuthError,
  AccountInfo,
  AuthenticationResult,
} from "@azure/msal-browser";

// MSAL configuration
const configuration: Configuration = {
  auth: {
    clientId: process.env.CLIENT_ID!,
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  }
};

export class AuthService {
  public clientApplication: PublicClientApplication;

  constructor() {
    this.clientApplication = new PublicClientApplication(configuration);
    this.clientApplication.initialize();
  }

  public getAccount(): AccountInfo | null {
    const accounts = this.clientApplication.getAllAccounts();
    const activeAccount = this.clientApplication.getActiveAccount();
    if (activeAccount) {
      return activeAccount;
    } else if (accounts.length > 0) {
      return accounts[0];
    } else {
      return null;
    }
  }

  public async getAccessToken(): Promise<string | undefined> {
    const request = {
      scopes: [`api://${process.env.CLIENT_ID}/access_as_user`],
    };

    let result : AuthenticationResult;
    try {
      result = await this.clientApplication.acquireTokenSilent(
        request
      );
      return result.accessToken;
    } catch (error) {
      // if our access / refresh / id token is expired we need redirect to AAD to get a new one.
      if (error instanceof InteractionRequiredAuthError) {
        await this.clientApplication.acquireTokenRedirect(request);
      }
    }
  }

  public async login(): Promise<void> {
    await this.clientApplication.loginRedirect();
  }

  public async setActiveAccount(): Promise<void> {
    const result = await this.clientApplication.handleRedirectPromise();
    if (result) {
      this.clientApplication.setActiveAccount(result.account);
    }
  }

  public async loginPopUp(): Promise<void> {
    const result = await this.clientApplication.loginPopup();
    this.clientApplication.setActiveAccount(result.account);
  }

  public logout(): Promise<void> {
    return this.clientApplication.logoutRedirect();
  }
}
