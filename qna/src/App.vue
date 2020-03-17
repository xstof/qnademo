<template>
  <div id="q-app">
    <router-view />
  </div>
</template>

<script>
import { ensureInitialized, getAccount } from './auth'

export default {
  name: 'App',

  // also see: https://quasar.dev/quasar-cli/cli-documentation/prefetch-feature
  preFetch ({ store }) {
    console.log('prefetch on App.vue: called.')

    // make sure authorization is initialized so that upon redirect callback from AAD B2C msal is able to call 'authRedirectCallBack'
    ensureInitialized()
    console.log('prefetch on App.vue: auth initialized')

    // initialize the store:
    console.log('prefetch on App.vue: initializing the vuex store')
    store.dispatch('qna/loadConfiguration')
      .then(() => {
        console.log('prefetch has initalized the vuex store')
      })

    // if user is logged in, then reflect this in the store:
    let account = getAccount()
    if (account) {
      console.log('prefetch on App.vue: user is logged in, putting this in store')
      store.commit('qna/setUser', { id: account.id, name: account.name, email: account.email })
    }
  }
}
</script>
