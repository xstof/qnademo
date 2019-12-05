import * as Msal from 'msal'

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
let clientId = 'd03fc97e-cc4e-4758-944a-43fe4cf3eecc'
let authorityForSignin = 'https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/b2c_1_susi'

console.log('setting msal redirect uri to:')
console.log(`${window.location.protocol}//${window.location.hostname}:${window.location.port}/login`)

function loggerCallback (logLevel, message, containsPii) {
  console.log(message)
}

let msalConfig = {
  auth: {
    clientId: clientId,
    authority: authorityForSignin,
    validateAuthority: false,
    redirectUri: `${window.location.protocol}//${window.location.hostname}:${window.location.port}/login`,
    postLogoutRedirectUri: `${window.location.protocol}//${window.location.hostname}:${window.location.port}`
    // navigateToLoginRequestUrl: false,
  },
  cache: {
    cacheLocation: 'localStorage'
  },
  system: {
    logger: new Msal.Logger(
      loggerCallback, {
        level: Msal.LogLevel.Verbose,
        piiLoggingEnabled: false,
        correlationId: '1234'
      }
    )
  }
}
var msalInstance = new Msal.UserAgentApplication(msalConfig)

msalInstance.handleRedirectCallback(authRedirectCallBack)

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
var loginRequest = {
  // scopes: ['openid', 'profile', 'email', authoringScope]
  scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation']
}

// redirect call back
function authRedirectCallBack (error, response) {
  console.log('authredirect callback executing')
  if (error) {
    console.log(error)
  } else {
    if (response.tokenType === 'id_token') {
      console.log('id token received')
      // acquireTokenRedirectAndCallMSGraph(graphConfig.graphMeEndpoint, loginRequest)
    } else if (response.tokenType === 'access_token') {
      // callMSGraph(graphConfig.graphMeEndpoint, response.accessToken, graphAPICallback)
    } else {
      console.log('token type is:' + response.tokenType)
    }
  }
  console.log('authredirect callback executed')
}

function login () {
  if (!msalInstance.getAccount() && !msalInstance.loginInProgress) {
    console.log('about to redirect using msal for a login flow')
    // msalInstance.redirectUri = `${baseRedirectUri}?returnto=${window.location.pathname}`
    msalInstance.loginRedirect(loginRequest)
  }
}

function logout () {
  msalInstance.logout()
}

var editProfileRequest = {
  scopes: ['openid', 'profile', 'email'],
  extraQueryParameters: { p: 'B2C_1_ep' }
}

function editProfile () {
  // msalInstance.redirectUri = `${baseRedirectUri}?returnto=${window.location.pathname}`
  console.log('about to redirect using msal for a profile edit flow')
  msalInstance.loginRedirect(editProfileRequest)
  login()
}

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
const accessTokenRequest = {
  // scopes: [authoringScope, 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],  => does not work with easyauth ?
  // scopes: [authoringScope],                                                             => does not work with easyauth ?
  // scopes: ['openid', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  scopes: ['https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  extraQueryParameters: { p: 'b2c_1_susi' }
}

function getToken () {
  return new Promise(function (resolve, reject) {
    if (msalInstance.getAccount()) {
      msalInstance.acquireTokenSilent(accessTokenRequest)
        .then(function (accessTokenResponse) {
          if (!accessTokenResponse.accessToken) {
            console.error(accessTokenResponse)
            // debugger
          }
          let accessToken = accessTokenResponse.accessToken
          console.log(`got access token: ${accessToken}`)
          resolve(accessToken)
        })
        .catch(function (error) {
          // console.log('failed acquiring an access token')
          // console.log(error)
          reject(error)
        })
    } else {
      reject('user not logged in - no token can be fetched')
    }
  })
}

export { msalInstance, login, logout, editProfile, getToken }
