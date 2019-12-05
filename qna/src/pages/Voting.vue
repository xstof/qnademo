<template>
  <q-page class="flex flex-center">
    <div class="q-pa-md row items-start q-gutter-md">
      <ParticipantVotingForm :questionToShow="currentQuestionToVoteOn" />
    </div>
  </q-page>
</template>

<script>
import ParticipantVotingForm from '../components/ParticipantVotingForm'
import { mapActions, mapGetters } from 'vuex'
import { subscribeToLiveUpdates } from '../liveupdates'

export default {
  name: 'Voting',
  created () {
    let store = this.$store
    let axios = this.$axios
    let route = this.$route
    this.$store.dispatch('qna/loadConfiguration')
      .then(() => {
        let sessionId = route.params.sessionId
        console.log(`getting latest session info for session: ${sessionId}`)
        axios.get(`api/sessions/${sessionId}/summary`)
          .then(function (response) {
            console.log('got latest session info:')
            console.log(response.data)
            store.commit('qna/setSessionDetails', { sessionName: response.data.sessionName, sessionId: response.data.sessionId })
            if (response.data.lastReleasedQuestion) {
              store.commit('qna/setCurrentQuestionToVoteOn', response.data.lastReleasedQuestion)
            }
            subscribeToLiveUpdates(axios, route.params.sessionId, store)
          })
      })
  },
  components: {
    ParticipantVotingForm
  },
  watch: {
    '$route.params.sessionId': function (id) {
      console.log('route params changed: ' + this.$route.params.sessionId)
      subscribeToLiveUpdates(this.$route.params.sessionId, this.$store)
    }
  },
  data () {
    return {
      localSessionName: this.$store.state.qna.session.name
    }
  },
  methods: {
    ...mapActions('qna', ['someAction'])
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress', 'currentQuestionToVoteOn'])
  }
}
</script>
