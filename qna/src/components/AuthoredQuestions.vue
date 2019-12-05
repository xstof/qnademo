<template>
  <q-list bordered>
    <q-item class="q-my-sm" clickable v-ripple
            @click="$emit('new-question')">
      <q-item-section>
          <q-item-label>New Question</q-item-label>
        </q-item-section>
    </q-item>
    <q-separator />
    <q-item v-for="question in submittedQuestions" :key="question.id" class="q-my-sm" clickable v-ripple
            @click="$emit('question-selected', question.id)">
        <q-item-section>
          <q-item-label>{{ question.title }}</q-item-label>
        </q-item-section>
        <q-item-section side>
          <q-icon v-if="question.isReleased" name="check_circle" color="green" @click="releaseQuestion(question)" />
          <q-btn v-else round color="primary" size="xs" icon="send" @click="releaseQuestion(question)"/>
        </q-item-section>
    </q-item>
  <q-separator />

  </q-list>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  name: 'AuthoredQuestions',
  data () {
    return {}
  },
  methods: {
    ...mapActions('qna', ['releaseQuestion']),
    startSession (e) {
      this.$store.commit('qna/startNewSessionWithName', this.localSessionName)
      this.$router.push({ path: 'author', params: { sessionId: 'test' } })
    }
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress', 'submittedQuestions'])
  }
}
</script>
