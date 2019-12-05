<template>
  <q-page class="flex flex-center">
    <div class="q-pa-md row items-start q-gutter-md">
      <q-card class="my-card bg-grey-9 text-white">
        <q-card-section>
          <div class="text-h6">Start a new QnA Session</div>
        </q-card-section>

        <q-card-section>
            <q-input
              filled
              v-model="localSessionName"
              label="Session Name"
              hint="session name"
              dark
              lazy-rules
              :rules="[ val => val && val.length > 0 || 'Please type something']"
            />

            <q-toggle label="I accept the license and terms" dark v-model="didAcceptTerms" />

            <div>
            </div>
        </q-card-section>

        <q-separator dark />

        <q-card-actions>
          <q-btn flat @click="startSession">Start Session</q-btn>
        </q-card-actions>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import { uuid } from 'vue-uuid'

export default {
  name: 'SessionCreate',
  data () {
    return {
      localSessionName: this.$store.state.qna.session.name,
      didAcceptTerms: false
    }
  },
  methods: {
    ...mapActions('qna', ['someAction']),
    startSession (e) {
      let newSessionId = uuid.v1()
      this.$store.dispatch('qna/startNewSessionWithName', { sessionName: this.localSessionName, sessionId: newSessionId })
      this.$router.push({ name: 'author', params: { sessionId: newSessionId } })
    }
  },
  computed: {
    ...mapGetters('qna', ['sessionName', 'isSessionInProgress'])
  }
}
</script>
