<template>
  <div>
    <div class="q-pa-md">
      <q-card :class="['my-card', isSubmitted ? 'light-dimmed' : '']">
        <q-card-section v-if="showQuestionToVoteOn">
          <div class="text-h6">{{questionToShow.title}}</div>
          <div class="text-subtitle2">Please submit your vote.</div>
        </q-card-section>
        <q-card-section v-else>
          <div class="text-h6">Please stand by</div>
          <div class="text-subtitle2">...until you moderator starts the session</div>
        </q-card-section>
        <q-separator />
        <q-markup-table class="q-my-md" v-if="showQuestionToVoteOn">
          <tbody>
            <tr v-for="answer in questionDetailsAnswerOptions"
               :key="answer.id"
               @click="selectAnswer(answer.id)">
              <td class="text-left" v-if="selectedAnswerId !== answer.id">
                <div>{{answer.title}}</div>
              </td>
              <td class="text-left bg-info" v-else>
                <div>{{answer.title}}</div>
              </td>
            </tr>
          </tbody>
        </q-markup-table>
        <q-separator v-if="showQuestionToVoteOn" />
        <q-card-actions align="center" v-if="showQuestionToVoteOn">
          <q-btn flat :disable="this.selectedAnswerId === null || this.selectedAnswerId === '' || this.isSubmitted"
                 @click="submitAnswer">Submit</q-btn>
        </q-card-actions>
      </q-card>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  name: 'ParticipantVotingForm',
  data () {
    return {
      selectedAnswerId: '',
      isSubmitted: false
    }
  },
  props: [
    'questionToShow'
  ],
  watch: {
    questionToShow: {
      handler (newValue, oldValue) {
        this.resetAnswer()
        this.resetSubmissionStatus()
      }
    }
  },
  methods: {
    ...mapActions('qna', ['someAction']),
    selectAnswer (answerId) {
      this.selectedAnswerId = answerId
    },
    submitAnswer () {
      let that = this
      let answerId = this.selectedAnswerId
      let baseUrl = 'api/sessions/{{sessionid}}/questions/{{questionid}}/answers/{{userid}}'
      // console.log(`user id is: ${this.userId}`)
      // console.log(`userOrBrowserSessionId is: ${this.userOrBrowserSessionId}`)
      let submissionUrl = baseUrl.replace('{{sessionid}}', this.sessionId)
        .replace('{{questionid}}', this.questionToShow.id)
        .replace('{{userid}}', this.userOrBrowserSessionId)
      console.log(`submitting answer ${this.selectedAnswerId} on url ${submissionUrl}`)
      this.$axios.post(submissionUrl,
        {
          answerId: answerId
        })
        .then(function (response) {
          console.log(`submitted answer ${answerId}`)
          that.isSubmitted = true
        })
    },
    resetAnswer () {
      this.selectedAnswerId = ''
    },
    resetSubmissionStatus () {
      this.isSubmitted = false
    }
  },
  computed: {
    ...mapGetters('qna', ['sessionId', 'sessionName', 'userId', 'userOrBrowserSessionId', 'isSessionInProgress', 'showQuestionToVoteOn']),
    questionDetailsAnswerOptions () {
      if (this.questionToShow === null || this.questionToShow === undefined) {
        return []
      } else {
        return this.questionToShow.answerOptions
      }
    }
  }
}
</script>
