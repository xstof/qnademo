<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <!-- <q-btn dense flat round icon="menu" @click="leftDrawerOpen = !leftDrawerOpen" /> -->
        <q-toolbar-title>Quick QnA ({{region}})</q-toolbar-title>
        <q-space />
        <!-- <q-btn stretch flat label="Login" @click="login" /> -->
        <q-btn-dropdown split color="primary" :label="labelForLoginButton" @click="login">
          <q-list>
            <q-item v-if="isSignedIn" clickable v-close-popup @click="editprofile" >
              <q-item-section>
                <q-item-label>Change Profile</q-item-label>
              </q-item-section>
            </q-item>

            <q-item v-if="isSignedIn" clickable v-close-popup @click="logout">
              <q-item-section>
                <q-item-label>Sign-Out</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </q-btn-dropdown>
      </q-toolbar>
    </q-header>

    <!-- <q-drawer show-if-above v-model="leftDrawerOpen" side="left" bordered> -->
    <!-- drawer content -->
    <!-- </q-drawer> -->

    <q-page-container>
      <q-dialog v-bind:value="isErrorActive" transition-show="scale" transition-hide="scale">
        <q-card class="bg-teal text-white" style="width: 300px">
          <q-card-section>
            <div class="text-h6">Oops... it looks like something went wrong.</div>
          </q-card-section>

          <q-card-section>{{errorMessage}}</q-card-section>

          <q-card-actions align="right" class="bg-white text-teal">
            <q-btn flat label="OK" v-close-popup @click="closeError" />
          </q-card-actions>
        </q-card>
      </q-dialog>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
import { mapGetters } from 'vuex'
import { login, logout, editProfile } from '../auth'

export default {
  name: 'MyLayout',

  data () {
    return {
      leftDrawerOpen: false
    }
  },

  methods: {
    closeError (e) {
      this.$store.commit('qna/setErrorActive', false)
    },
    login () {
      login()
    },
    logout () {
      logout()
    },
    editprofile () {
      editProfile()
    }
  },

  created () {
    // this.$store.dispatch('qna/loadConfiguration')
  },

  computed: {
    ...mapGetters('qna', ['region', 'isErrorActive', 'errorMessage', 'userName', 'isSignedIn']),
    labelForLoginButton: function () {
      if (this.$store.state.qna.isSignedIn === true) {
        return this.$store.state.qna.user.name
      } else {
        return 'Login'
      }
    }
  }
}
</script>
