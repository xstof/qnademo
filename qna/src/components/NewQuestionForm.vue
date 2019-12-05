<template>
  <q-form
    @submit="submitNewQuestion"
    @reset="resetForm"
    class="q-gutter-md"
  >
    <q-input
      filled
      v-model="newQuestion.title"
      label="Question Phrase"
      lazy-rules
      :rules="[ val => val && val.length > 0 || 'Please type something']"
    />
    <q-separator/>
    <q-input class="q-my-xs"
      dense
      filled
      v-model="newQuestion.answerOptions[0].title"
      label="Answer Option 1"
      lazy-rules
      :rules="[ val => val && val.length > 0 || 'Please type something']"
    />
    <q-input class="q-my-xs"
      dense
      filled
      v-model="newQuestion.answerOptions[1].title"
      label="Answer Option 2"
      lazy-rules
      :rules="[ val => val && val.length > 0 || 'Please type something']"
    />
    <q-input class="q-my-xs"
      dense
      filled
      v-model="newQuestion.answerOptions[2].title"
      label="Answer Option 3"
      lazy-rules
      :rules="[ val => val && val.length > 0 || 'Please type something']"
    />
    <q-input class="q-my-xs"
      dense
      filled
      v-model="newQuestion.answerOptions[3].title"
      label="Answer Option 4"
      lazy-rules
      :rules="[ val => val && val.length > 0 || 'Please type something']"
    />
    <div>
      <q-btn label="Submit" type="submit" color="primary"/>
      <q-btn label="Reset" type="reset" color="primary" flat class="q-ml-sm" />
    </div>
  </q-form>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import { uuid } from 'vue-uuid'

export default {
  name: 'NewQuestionForm',
  data () {
    return {
      newQuestion: {
        title: '',
        isReleased: false,
        id: uuid.v1(),
        answerOptions: [
          {
            id: '1',
            title: ''
          },
          {
            id: '2',
            title: ''
          },
          {
            id: '3',
            title: ''
          },
          {
            id: '4',
            title: ''
          }
        ]
      }
    }
  },
  methods: {
    ...mapActions('qna', ['someAction']),
    submitNewQuestion (e) {
      let copyOfQuestion = JSON.parse(JSON.stringify(this.newQuestion))
      this.$store.dispatch('qna/submitNewQuestion', copyOfQuestion)
      this.newQuestion.id = uuid.v1()
    },
    resetForm () {
      this.newQuestion.id = uuid.v1()
      this.newQuestion.isReleased = false
      this.newQuestion.title = ''
      this.newQuestion.answerOptions[0].title = ''
      this.newQuestion.answerOptions[1].title = ''
      this.newQuestion.answerOptions[2].title = ''
      this.newQuestion.answerOptions[3].title = ''
    }
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress', 'submittedQuestions'])
  }
}
</script>
