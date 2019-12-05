import Vue from 'vue'

export function setErrorActive (state, errorState) {
  state.error.active = errorState
}

export function showError (state, errorMessage) {
  state.error.active = true
  state.error.message = errorMessage
}

export function setConfigRegion (state, region) {
  state.configuration.region = region
}

export function setConfigSignalRNegotiateUrl (state, negotiateUrl) {
  state.configuration.signalRNegotiateUrl = negotiateUrl
}

export function setConfigBrowserSessionId (state, browserSessionId) {
  state.configuration.browserSessionId = browserSessionId
}

export function setUser (state, userDetails) {
  state.isSignedIn = true
  state.user.id = userDetails.id
  state.user.name = userDetails.name
  state.user.email = userDetails.email
}

export function setSessionDetails (state, sessionParams) {
  state.session.isStarted = true
  state.session.name = sessionParams.sessionName
  state.session.id = sessionParams.sessionId
}

export function clearQuestions (state) {
  state.submittedQuestions = []
}

export function addSubmittedQuestion (state, question) {
  state.submittedQuestions.push(question)
}

export function releaseQuestion (state, question) {
  let questionToRelease = state.submittedQuestions.find((q) => q.id === question.id)
  questionToRelease.isReleased = true
}

export function setCurrentQuestionToVoteOn (state, question) {
  // state.currentQuestionToVoteUpon = question
  Vue.set(state, 'currentQuestionToVoteUpon', question)
}

export function updateResponses (state, sessionResult) {
  state.responses = []
  sessionResult.questions.forEach(question => {
    state.responses.push({
      questionId: question.questionId,
      answers: question.answerOptions.map(o => {
        return {
          answerOptionId: o.id,
          count: o.voteCount
        }
      })
    })
  })
  console.log('updated session results:')
  console.log(sessionResult)
}
