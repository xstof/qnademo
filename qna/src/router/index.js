import Vue from 'vue'
import VueRouter from 'vue-router'
// import { getAccount, login } from '../auth'

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

  // Router.beforeEach((to, from, next) => {
  //   var account = getAccount()
  //   if (account) {
  //     store.commit('qna/setUser', { id: account.id, name: account.name, email: account.email })
  //     console.log('user was already logged in; committed user details to store')
  //     next()
  //   } else if (to.matched.some(record => record.meta.requiresAuth)) {
  //     console.log('path being navigated to requires being logged in - redirecting now')
  //     login()
  //     // next({
  //     //   path: '/login',
  //     //   query: { returnto: to.path },
  //     //   params: { nextUrl: to.fullPath }
  //     // })
  //     next()
  //   } else {
  //     next()
  //   }
  // })

  return Router
}
