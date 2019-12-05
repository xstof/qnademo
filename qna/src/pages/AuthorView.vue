<template>
  <q-page class="q-pa-md" >
    <div class="row">
      <q-toolbar class="text-primary bg-grey-3 q-my-md">
        <q-btn flat @click="toggleShowQRCode"> Toggle QR Code </q-btn>
      </q-toolbar>
    </div>
    <div class="row" v-if="!showQRCode">
      <div class="col-4">
        <AuthoredQuestions v-on:question-selected="showQuestionDetails($event)"
                           v-on:new-question="showAuthorView()"/>
      </div>
      <div class="col-8 q-pl-md">
        <NewQuestionForm v-if="showNewQuestionForAuthoring"/>
        <SubmittedQuestionDetails v-if="!showNewQuestionForAuthoring" :questionIdToShow="selectedQuestion" />
      </div>
    </div>
    <div class="row justify-center items-center window-height window-width text-center q-py-md" v-else>
      <qrcode class="full-height q-my-sm" :value="participantUrl"/>
    </div>
  </q-page>
</template>

<script>
import Qrcode from 'vue-qrcode'
import AuthoredQuestions from '../components/AuthoredQuestions'
import NewQuestionForm from '../components/NewQuestionForm'
import SubmittedQuestionDetails from '../components/SubmittedQuestionDetails'
import { subscribeToLiveUpdates } from '../liveupdates'

import { mapActions, mapGetters } from 'vuex'

export default {
  name: 'AuthorView',
  components: {
    AuthoredQuestions,
    NewQuestionForm,
    SubmittedQuestionDetails,
    Qrcode
  },
  data () {
    return {
      localSessionName: this.$store.state.qna.session.name,
      showNewQuestionForAuthoring: true,
      didAcceptTerms: false,
      selectedQuestion: null,
      showQRCode: false
    }
  },
  created () {
    this.fetchData()
    let store = this.$store
    let axios = this.$axios
    let route = this.$route
    let sessionId = route.params.sessionId
    this.$store.dispatch('qna/loadConfiguration')
      .then(() => {
        console.log(`getting latest session info for session: ${sessionId}`)
        axios.get(`api/sessions/${sessionId}/summary`)
          .then(function (response) {
            // console.log('got latest session info:')
            // console.log(response.data)
            store.commit('qna/setSessionDetails', { sessionName: response.data.sessionName, sessionId: response.data.sessionId })
            if (response.data.lastReleasedQuestion) {
              store.commit('qna/setCurrentQuestionToVoteOn', response.data.lastReleasedQuestion)
            }
            subscribeToLiveUpdates(axios, route.params.sessionId, store)
          })
      })
      .then(() => {
        // console.log(`getting session results for session: ${sessionId}`)
        this.$store.dispatch('qna/fetchSessionResultsFromBackend', sessionId)
      })
  },
  watch: {
    '$route': 'fetchData'
  },
  methods: {
    ...mapActions('qna', ['someAction']),
    showAuthorView () {
      this.showNewQuestionForAuthoring = true
    },
    showQuestionDetails (questionId) {
      this.selectedQuestion = questionId
      this.showNewQuestionForAuthoring = false
    },
    fetchData () {
      this.$store.dispatch('qna/fetchSessionFromBackend', this.$route.params.sessionId)
    },
    toggleShowQRCode () {
      if (this.showQRCode) {
        this.showQRCode = false
      } else {
        this.showQRCode = true
      }
    }
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress']),
    participantUrl () {
      return `${window.location.protocol}/${window.location.hostname}:${window.location.port}/session/${this.$route.params.sessionId}`
    }
  }
}
</script>
