export function isErrorActive (state) {
  return state.error.active
}

export function errorMessage (state) {
  return state.error.message
}

export function userName (state) {
  return state.user.name
}

export function userId (state) {
  return state.user.id
}

export function browserSessionId (state) {
  return state.configuration.browserSessionId
}

export function userOrBrowserSessionId (state) {
  if (userId(state)) {
    return userId(state)
  } else {
    return browserSessionId(state)
  }
}

export function isSignedIn (state) {
  return state.isSignedIn
}

export function region (state) {
  return state.configuration.region
}

export function signalRSubscribeUrl (state) {
  return ({ sessionId, userId }) => {
    let baseUrl = state.configuration.signalRNegotiateUrl
    let subscribeUrl = baseUrl.replace('{sessionId}', sessionId).replace('{userId}', userId)
    return subscribeUrl
  }
}

export function sessionId (state) {
  return state.session.id
}

export function sessionName (state) {
  return state.session.name
}

export function isSessionInProgress (state) {
  return state.session.isStarted
}

export function submittedQuestions (state) {
  return state.submittedQuestions
}

export function responses (state) {
  return (questionId) => {
    let resp = state.responses.find(r => r.questionId === questionId)
    if (resp === null || resp === undefined) {
      return []
    } else {
      return resp.answers
    }
  }
}

export function totalResponseCount (state) {
  return (questionId) => {
    let resp = state.responses.find(r => r.questionId === questionId)
    if (resp === null || resp === undefined) {
      return 0
    } else {
      return resp.answers.reduce((acc, curr) => acc + curr.count, 0)
    }
  }
}

export function currentQuestionToVoteOn (state) {
  return state.currentQuestionToVoteUpon
}

export function showQuestionToVoteOn (state) {
  console.log(state.currentQuestionToVoteOn)
  let question = state.currentQuestionToVoteUpon
  if (question) {
    if (question.isReleased) {
      return true
    }
  }
  return false
}
