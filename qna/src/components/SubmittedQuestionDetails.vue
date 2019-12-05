<template>
  <div>
    <q-banner rounded class="bg-grey-3">
      {{questionDetails.title}}
    </q-banner>
    <q-markup-table class="q-my-md">
      <tbody>
        <tr v-for="answer in questionDetailsAnswerOptions" :key="answer.id">
          <td class="text-left">
            <div>{{answer.title}}</div>
            <div>{{answer.count}}</div>
            <q-linear-progress v-if="(countOfResponsesTo(answer.id) > 0)"
                               :value="(countOfResponsesTo(answer.id)/totalResponseCount)"
                               color="secondary" class="q-mt-sm" />
            <q-linear-progress v-else
                               :value="0"
                               color="secondary" class="q-mt-sm" />
          </td>
        </tr>
      </tbody>
    </q-markup-table>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  name: 'SubmittedQuestionDetails',
  data () {
    return {
    }
  },
  props: [
    'questionIdToShow'
  ],
  methods: {
    ...mapActions('qna', ['someAction'])
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress', 'submittedQuestions', 'totalResponseCount']),
    questionDetails () {
      return this.submittedQuestions.find(q => q.id === this.questionIdToShow)
    },
    questionDetailsAnswerOptions () {
      let question = this.submittedQuestions.find(q => q.id === this.questionIdToShow)
      if (question === null || question === undefined) {
        return []
      } else {
        return question.answerOptions
      }
    },
    responses () {
      return this.$store.getters['qna/responses'](this.questionIdToShow)
    },
    countOfResponsesTo (answerOptionId) {
      return (answerOptionId) => {
        let responses = this.$store.getters['qna/responses'](this.questionIdToShow)
        let option = responses.find(r => r.answerOptionId === answerOptionId)
        if (option === null || option === undefined) {
          return 0
        } else {
          return option.count
        }
      }
    },
    totalResponseCount () {
      return this.$store.getters['qna/totalResponseCount'](this.questionIdToShow)
    }
  }
}
</script>
