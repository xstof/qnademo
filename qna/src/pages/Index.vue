<template>
  <!-- <q-page class="flex flex-center"> -->
    <q-page>
    <!-- <img alt="Quasar logo" src="~assets/quasar-logo-full.svg"> -->

    <div class="column items-center justify-center q-ma-lg window-height">
      <!-- <div class="col" /> -->
      <div class="col-auto">
        <q-btn size="md" class="full-width" @click="navigateToCreateNewSession">
          <q-icon name="create" class="q-mx-md" />
          <div>Start a new session</div>
        </q-btn>
      </div>
      <div class="col-auto">
        <q-input outlined bottom-slots v-model="sessionId" label=" ... or join a session" class="q-my-md">
          <!-- <template v-slot:before>
            <q-icon name="slideshow" />
          </template> -->

          <template v-slot:hint>
            Enter the identifier you were given
          </template>

          <template v-slot:append>
            <q-btn round dense flat icon="play_circle_filled" @click="navigateToSessionAsParticipant" />
          </template>
        </q-input>
      </div>
    </div>
  </q-page>
</template>

<script>
export default {
  name: 'PageIndex',
  data () {
    return {
      sessionId: ''
    }
  },
  methods: {
    navigateToCreateNewSession () {
      this.$router.push({ name: 'createsession' })
    },
    navigateToSessionAsParticipant () {
      this.$router.push({ name: 'participant', params: { sessionId: this.sessionId } })
    }
  }
}
</script>
