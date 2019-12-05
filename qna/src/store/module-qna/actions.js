import { exponentialDelay } from 'axios-retry'

export function loadConfiguration (context) {
  return new Promise((resolve, reject) => {
    this._vm.$axios.get('api/configuration')
      .then(function (response) {
        console.log('configuration loaded:')
        console.log(response.data)
        context.commit('setConfigRegion', response.data.region)
        context.commit('setConfigSignalRNegotiateUrl', response.data.signalRNegotiateUrl)
        context.commit('setConfigBrowserSessionId', response.data.browserSessionId)
        // let config = context.state.configuration
        // console.log(config)
        resolve()
      })
      .catch(function (error) {
        context.commit('showError', 'Failed to load configuration from backend api.')
        reject(error)
      })
  })
}

export function startNewSessionWithName (context, sessionParams) {
  this._vm.$axios.post('api/CreateSession',
    {
      id: sessionParams.sessionId,
      name: sessionParams.sessionName
    }
  ).then(function (response) { context.commit('setSessionDetails', sessionParams) }).catch(function (error) {
    console.error('failed to create session')
    console.log(sessionParams)
    throw error
  })
}

export function fetchSessionFromBackend (context, sessionId) {
  this._vm.$axios.get(`api/sessions/${sessionId}`,
    {
      'axios-retry': {
        retries: 8,
        delay: exponentialDelay,
        retryCondition: (error) => {
          console.log(error)
          return true
        }
      }
    })
    .then(function (response) {
      // console.log('fetched session infromation from api backend')
      context.commit('clearQuestions')
      // console.log('cleared questions from store')
      // refresh session info
      context.commit('setSessionDetails', { sessionName: response.data.sessionName, sessionId: response.data.sessionId })
      console.log('refreshed session info from api backend')
      // refresh questions
      response.data.questions.forEach(question => {
        context.commit('addSubmittedQuestion', question)
      })
      console.log('added questions to store from api backend')
    })
    .catch(function (error) {
      console.error('failed fetching session information from api backend')
      context.commit('showError', 'Failed to load session from backend api.')
      console.log(error)
    })
}

export function fetchSessionResultsFromBackend (context, sessionId) {
  this._vm.$axios.get(`api/sessions/${sessionId}/results`,
    {
      'axios-retry': {
        retries: 8,
        delay: exponentialDelay,
        retryCondition: (error) => {
          console.log(error)
          return true
        }
      }
    })
    .then(function (response) {
      console.log('fetched session results from api backend')
      // console.log(response.data)
      context.commit('updateResponses', response.data)
    })
}

export function someAction (/* context */) {
}

export function submitNewQuestion (context, newQuestion) {
  let sessionId = context.state.session.id
  console.log('submitting new question for session with id: ' + sessionId)

  this._vm.$axios.post(`api/sessions/${sessionId}/questions`, newQuestion)
    .then(function (response) {
      console.log('new question submitted')
      context.commit('addSubmittedQuestion', newQuestion)
    })
    .catch(function (error) { console.log(error) })
}

export function releaseQuestion (context, question) {
  let sessionId = context.state.session.id
  this._vm.$axios.post(`api/sessions/${sessionId}/questions/${question.id}/release`)
    .then(function () {
      console.log(`question released with id: ${question.id}`)
      context.commit('releaseQuestion', question)
    })
    .catch(function (error) { console.log(error) })
}
