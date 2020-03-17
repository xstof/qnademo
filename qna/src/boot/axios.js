import Vue from 'vue'
import axios from 'axios'
import axiosRetry from 'axios-retry'
import { getToken } from '../auth'

// We create our own axios instance and set a custom base URL.
// Note that if we wouldn't set any config here we do not need
// a named export, as we could just `import axios from 'axios'`
// TODO - make this URL dynamic

// console.log(`axiosInstance being created - ${window.location.protocol}//${window.location.hostname}//`)

var frontendurl = 'https://qnaqa-frontdoor.azurefd.net/'
if (process.env.FRONTEND_URL) {
  frontendurl = process.env.FRONTEND_URL
}
console.log(`axiosInstance being created: ${frontendurl}`)
const axiosInstance = axios.create({
  // do not hardcode frontend url, use quasar variables instead
  // see: https://quasar.dev/quasar-cli/cli-documentation/handling-process-env#Example
  baseURL: frontendurl
  // baseURL: 'https://qnaqa-frontdoor.azurefd.net/'
})

axiosInstance.interceptors.request.use(
  config => {
    console.log(`url requested: ${config.url}`)
    if (config.url !== 'api/configuration') {
      console.log(`trying to get token to attach to auth header for url: ${config.url}`)

      return getToken().then(function (token) {
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
          console.log(`added bearer token to auth header for url: ${config.url}`)
        }
        return config
      }).catch(function (error) {
        console.log(`failed fetching token for url: ${config.url}`)
        let e = error
        if (e) {}
        return config
      })
    } else {
      console.log('making request to api/configuration - no need for token')
      return config
    }
  }, error => {
    Promise.reject(error)
  }
)

// Attach axios-retry for retrying HTTP calls
axiosRetry(axiosInstance, {
  retries: 5,
  retryDelay: axiosRetry.exponentialDelay
})

// for use inside Vue files through this.$axios
Vue.prototype.$axios = axiosInstance

// Here we define a named export
// that we can later use inside .js files:
export { axiosInstance }
