export default {
  configuration: {
    region: '',
    auth: {
      clientId: 'd03fc97e-cc4e-4758-944a-43fe4cf3eecc',
      authority: 'https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/b2c_1_susi',
      authoringScope: 'https://xstofb2c.onmicrosoft.com/qna/qna_author'
    },
    browserSessionId: 'id used instead of cookies to avoid cross domain config - when anonymous this is used as an artificial user name',
    signalRNegotiateUrl: 'negotiate url is region-based so needs to come from backend'
  },
  error: {
    active: false,
    message: 'this is an error message'
  },
  isSignedIn: false,
  user: {
    id: '',
    name: '',
    email: ''
  },
  session: {
    isStarted: false,
    name: '',
    id: ''
  },
  selectedQuestionId: null,
  submittedQuestions: [
    {
      title: 'This is the first DUMMY question',
      isReleased: true,
      id: '1',
      answerOptions: [
        {
          id: '1',
          title: 'answer option 1 - from first question'
        },
        {
          id: '2',
          title: 'answer option 2 - from first question'
        }
      ]
    },
    {
      title: 'This is the second DUMMY question',
      id: '2',
      isReleased: false,
      answerOptions: [
        {
          id: '1',
          title: 'answer option 1 - from second question'
        },
        {
          id: '2',
          title: 'answer option 2 - from second question'
        }
      ]
    },
    {
      title: 'This is the third DUMMY question',
      id: '3',
      isReleased: false,
      answerOptions: [
        {
          id: '1',
          title: 'answer option 1 - from third question'
        },
        {
          id: '2',
          title: 'answer option 2 - from third question'
        }
      ]
    }
  ],
  currentQuestionToVoteUpon: {
    title: 'This is the first DUMMY question to vote upon',
    isReleased: false,
    id: '1',
    answerOptions: [
      {
        id: '1',
        title: 'answer option 1 - from first question'
      },
      {
        id: '2',
        title: 'answer option 2 - from first question'
      }
    ]
  },
  responses: [
    {
      questionId: '1',
      answers: [
        {
          answerOptionId: '1',
          count: 4
        },
        {
          answerOptionId: '2',
          count: 8
        }
      ]
    }
  ]
}
