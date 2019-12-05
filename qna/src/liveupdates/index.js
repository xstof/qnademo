import { Notify } from 'quasar'
import { HubConnectionBuilder } from '@aspnet/signalr'

function subscribeToLiveUpdates (axios, sessionId, store) {
  console.log(`about to connect to signalr for session id: ${sessionId}`)

  let userId = store.state.qna.configuration.browserSessionId
  console.log(`connecting to signalr as user with id: ${userId}`)
  let baseUrl = store.state.qna.configuration.signalRNegotiateUrl
  let individualizedBaseUrl = baseUrl.replace('{sessionId}', sessionId).replace('{userId}', userId)
  let subscribeUrl = `${individualizedBaseUrl}/subscribe`
  let negotiateUrl = `${individualizedBaseUrl}`
  // console.log(`subscribing to signalr updates on url: ${subscribeUrl}`)
  // console.log(`negotiating signalr connection on url: ${negotiateUrl}`)

  axios.post(subscribeUrl)
    .then(function (response) {
      console.log(response)
      console.log(`subscribed to session ${sessionId} - will negotiate signalr on: ${negotiateUrl}`)

      const connection = new HubConnectionBuilder()
        .withUrl(negotiateUrl)
        // .configureLogging(signalR.LogLevel.Information)
        // .withAutomaticReconnect([2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5])
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: retryContext => {
            console.log('providing next retry attempt')
            return 2 * 1000
          } })
        .build()

      connection.onreconnecting((error) => {
        console.log(`signalr lost connection - trying to reconnect: ${error}`)
        Notify.create({ color: 'warning', message: 'SignalR connection lost - trying to reconnect', type: 'warning' })
      })

      connection.onreconnected((connectionId) => {
        console.log('signal connection reestablished - connected')
        Notify.create({ color: 'positive', message: 'SignalR connection reestablished.  Connected.', type: 'positive' })
      })

      connection.on('sessionUpdateForParticipant', session => {
        let currQuestion = store.getters['qna/currentQuestionToVoteOn']
        // console.log(`curr question id: ${currQuestion.id}`)
        // console.log(`incoming session last released question id: ${session.lastReleasedQuestion.id}`)
        // console.log('new session update for participant came in')
        // console.log(session)
        if (session.lastReleasedQuestion) {
        // only update if question being release is a new one:
          if (currQuestion.id !== session.lastReleasedQuestion.id) {
            console.log(`updating store with new question to vote upon - question id: ${session.lastReleasedQuestion.id}`)
            store.commit('qna/setCurrentQuestionToVoteOn', session.lastReleasedQuestion)
          }
        }
      })

      connection.on('sessionResultsUpdate', sessionResult => {
        // console.log(`session result update:`)
        // console.log(sessionResult)
        store.commit('qna/updateResponses', sessionResult)
      })

      connection.onclose(() => {
        console.log('disconnected')
        Notify.create({ color: 'negative', message: 'SignalR disconnected', type: 'warning' })
      })
      console.log('connecting...')

      connection.start()
        .then(() => {
          console.log('signalr connection started')
          Notify.create({ color: 'positive', message: 'SignalR connected', type: 'positive' })
        })
        .catch(console.error)
    })
}

export { subscribeToLiveUpdates }
