import Vue from 'vue'
import Vuex from 'vuex'

import qna from './module-qna'

Vue.use(Vuex)

/*
 * If not building with SSR mode, you can
 * directly export the Store instantiation
 */

export default function (/* { ssrContext } */) {
  const Store = new Vuex.Store({
    modules: {
      // example
      qna
    },

    // enable strict mode (adds overhead!)
    // for dev mode only
    strict: process.env.DEV
  })

  // initialize store with basic config from backend:
  // Store.dispatch('qna/loadConfiguration')

  return Store
}
