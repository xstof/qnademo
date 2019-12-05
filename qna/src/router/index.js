import Vue from 'vue'
import VueRouter from 'vue-router'
import { msalInstance, login } from '../auth'

import routes from './routes'

Vue.use(VueRouter)

/*
 * If not building with SSR mode, you can
 * directly export the Router instantiation
 */

export default function ({ store }) { /* could also have ssrContext injected here */
  const Router = new VueRouter({
    scrollBehavior: () => ({ x: 0, y: 0 }),
    routes,

    // Leave these as is and change from quasar.conf.js instead!
    // quasar.conf.js -> build -> vueRouterMode
    // quasar.conf.js -> build -> publicPath
    mode: process.env.VUE_ROUTER_MODE,
    base: process.env.VUE_ROUTER_BASE
  })

  Router.beforeEach((to, from, next) => {
    if (msalInstance.getAccount()) {
      let account = msalInstance.getAccount()
      let email = ''
      if (account.idToken.emails) {
        email = account.idToken.emails[0]
        console.log(`email is ${email}`)
      }
      store.commit('qna/setUser', { id: account.accountIdentifier, name: account.name, email: email })
      console.log('user was already logged in; committed user details to store')
      next()
    } else if (to.matched.some(record => record.meta.requiresAuth)) {
      login()
      // next({
      //   path: '/login',
      //   query: { returnto: to.path },
      //   params: { nextUrl: to.fullPath }
      // })
    } else {
      next()
    }
  })

  return Router
}
