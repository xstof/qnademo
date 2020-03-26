// import something here
import { ensureInitialized, getAccount, login } from '../auth'

// "async" is optional
export default async ({ router, store }) => {
  console.log('authboot being called')

  console.log('authboot: initializing the vuex store')
  await store.dispatch('qna/loadConfiguration')

  console.log('authboot: initalized the vuex store')
  // make sure authorization is initialized so that upon redirect callback from AAD B2C msal is able to call 'authRedirectCallBack'
  console.log('authboot: initializing auth')
  ensureInitialized()
  console.log('authboot: auth initialized')

  // if user is logged in, then reflect this in the store:
  let account = getAccount()
  if (account) {
    console.log('authboot: user is logged in, putting this in store')
    store.commit('qna/setUser', { id: account.id, name: account.name, email: account.email })
  }

  // configure router:
  console.log('authboot: configuring router')
  router.beforeEach((to, from, next) => {
    var account = getAccount()
    if (account) {
      store.commit('qna/setUser', { id: account.id, name: account.name, email: account.email })
      console.log('router: user was already logged in; committed user details to store')
      next()
    } else if (to.matched.some(record => record.meta.requiresAuth)) {
      console.log('router: path being navigated to requires being logged in - redirecting now')
      login()
      // next({
      //   path: '/login',
      //   query: { returnto: to.path },
      //   params: { nextUrl: to.fullPath }
      // })
      next()
    } else {
      next()
    }
  })
}
