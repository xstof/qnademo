# Quasar App (qna)

A Quasar Framework app

## Install the dependencies
```bash
npm install
```

## Change the app to work with your AAD B2C tenant

Until this is cleaned up there's some actions to be done unfortunately.

First of all makes sure to create an AAD B2C tenant and register your apps in there.
Then change the `./auth/index.js` file to reflect your app registration and your AAD B2C tenant:

~~~js
// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
let clientId = 'd03fc97e-cc4e-4758-944a-43fe4cf3eecc'
let authorityForSignin = 'https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/b2c_1_susi'

// ...

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
var loginRequest = {
  // scopes: ['openid', 'profile', 'email', authoringScope]
  scopes: ['openid', 'profile', 'email', 'https://xstofb2c.onmicrosoft.com/qna/user_impersonation']
}

// ...

// TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
const accessTokenRequest = {
  scopes: ['https://xstofb2c.onmicrosoft.com/qna/user_impersonation'],
  extraQueryParameters: { p: 'b2c_1_susi' }
}
~~~

Then also update `state.js` to do the same.

## Start the app in development mode (hot-code reloading, error reporting, etc.)
```bash
quasar dev
```

## Build the app for production
```bash
quasar build
```

## Deploy the application to the storage containers

```ps1
cd ../deployment
deploy-static-assets-global.ps1 
```
