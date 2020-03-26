import * as Msal from 'msal'
import store from './../store/module-qna'

// let clientId = 'd03fc97e-cc4e-4758-944a-43fe4cf3eecc'
// let authorityForSignin = 'https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/b2c_1_susi'

let clientId = ''
let authorityForSignin = ''

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
    // navigateToLoginRequestUrl: false
  },
  cache: {
    cacheLocation: 'localStorage'
  },
  system: {
    logger: new Msal.Logger(
      loggerCallback, {
        level: Msal.LogLevel.Verbose,
        piiLoggingEnabled: true,
        correlationId: '1234'
      }
    )
  }
}

var msalInstance = null
var isInitialized = false

function ensureInitialized () {
  if (!isInitialized) {
    if (store.state) {
      // console.log('initializing auth - fetching auth configuration from store:')
      // console.log('- old msal config:')
      // console.log(JSON.stringify(msalConfig.auth))
      var authConfig = store.state.configuration.auth
      // console.log('- fetched config: ')
      // console.log(JSON.stringify(store.state.configuration.auth))
      msalConfig.auth.clientId = authConfig.clientId
      msalConfig.auth.authority = authConfig.authority
      // console.log('initialized auth - new msal config:')
      // console.log(JSON.stringify(msalConfig.auth))

      // initialize msalInstance with updated msalConfig
      msalInstance = new Msal.UserAgentApplication(msalConfig)
      msalInstance.handleRedirectCallback(authRedirectCallBack)

      // initialization finished
      isInitialized = true
    }
  }
}

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
var loginRequest = {
  // scopes: ['openid', 'profile', 'email', authoringScope]
  // scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation']
  // scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/qna_author']
  scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/qna_author', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation']
}

// redirect call back
function authRedirectCallBack (error, response) {
  console.log('authredirect callback executing')

  if (error) {
    console.log(error)
  } else {
    var acct = window.localStorage.getItem('acct')
    if (!acct) {
      // below line is a workaround for bug: https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/1308
      console.log('seeing account for first time - storing it to fetch access tokens with later on')
      window.localStorage.setItem('acct', JSON.stringify(msalInstance.getAccount()))
      var acctStored = JSON.parse(window.localStorage.getItem('acct'))
      console.log(acctStored)
    }

    if (response.tokenType === 'id_token') {
      console.log('id token received')
    } else if (response.tokenType === 'access_token') {
    } else {
      console.log('token type is:' + response.tokenType)
    }
  }
  console.log('authredirect callback executed')
}

function shouldLogin () {
  var acct = msalInstance.getAccount()
  var isLoginInProgress = msalInstance.getLoginInProgress()
  if (!acct) {
    if (!isLoginInProgress) {
      return true
    }
  }
  return false
}

function login () {
  ensureInitialized()
  if (shouldLogin()) {
    console.log('about to redirect using msal for a login flow')
    msalInstance.loginRedirect(loginRequest)
  }
}

function logout () {
  window.localStorage.removeItem('acct')
  msalInstance.logout()
}

var editProfileRequest = {
  scopes: ['openid', 'profile', 'email'],
  // scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/qna_author'],
  // scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/qna_author', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  extraQueryParameters: { p: 'B2C_1_ep' }
}

function editProfile () {
  ensureInitialized()
  console.log('about to redirect using msal for a profile edit flow')
  msalInstance.loginRedirect(editProfileRequest)
}

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
var accessTokenRequest = {
  // scopes: [authoringScope, 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],  => does not work with easyauth ?
  // scopes: [authoringScope],                                                             => does not work with easyauth ?
  // scopes: ['openid', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  // scopes: ['https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  // scopes: ['https://xstofb2c.onmicrosoft.com/qna/qna_author'],
  scopes: ['https://xstofb2c.onmicrosoft.com/qna/qna_author', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  extraQueryParameters: { p: 'b2c_1_susi' },
  account: null
}

function getToken () {
  ensureInitialized()
  return new Promise(function (resolve, reject) {
    if (msalInstance.getAccount()) {
      var acct = JSON.parse(window.localStorage.getItem('acct'))
      accessTokenRequest.account = acct
      // console.log('access token request parameters:')
      // console.log(accessTokenRequest)
      msalInstance.acquireTokenSilent(accessTokenRequest)
        .then(function (accessTokenResponse) {
          if (!accessTokenResponse.accessToken) {
            console.error(accessTokenResponse)
          }
          let accessToken = accessTokenResponse.accessToken
          console.log(`auth: got access token: ${accessToken}`)
          resolve(accessToken)
        })
        .catch(function (error) {
          console.log(`auth: acquireTokenSilent failed - might need interaction`)
          if (error.errorMessage.indexOf('interaction_required') !== -1 || error.errorMessage.indexOf('InteractionRequiredAuthError') !== -1) {
            msalInstance.acquireTokenRedirect(accessTokenRequest)
          } else {
            console.log('auth: no indication interaction was required - failed acquiring an access token')
            console.log(error)
            reject(error)
          }
        })
    } else {
      reject('auth: user not logged in - no token can be fetched')
    }
  })
}

function getAccount () {
  var account = null

  if (msalInstance && msalInstance.getAccount()) {
    account = {
      id: '',
      name: '',
      email: ''
    }
    var msalAccount = msalInstance.getAccount()
    account.id = msalAccount.accountIdentifier
    account.name = msalAccount.name
    if (msalAccount.idToken && msalAccount.idToken.emails) {
      account.email = msalAccount.idToken.emails[0]
    }
  }

  return account
}

export { ensureInitialized, getAccount, login, logout, editProfile, getToken }
