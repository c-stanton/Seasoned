<template>
  <v-dialog v-model="showTimeout" persistent max-width="450">
    <v-card class="recipe-card pa-6 text-center" theme="light">
      <div class="mb-4">
        <v-icon color="#8c4a32" size="64">mdi-clock-outline</v-icon>
      </div>
      
      <h2 class="auth-title mb-2">Session Expired</h2>
      
      <v-card-text class="text-body-1" style="font-family: 'Libre Baskerville', serif;">
        Your kitchen session has timed out after 30 minutes of inactivity. 
        Please sign back in to continue managing your recipes.
      </v-card-text>

      <v-divider class="my-4 separator"></v-divider>

      <v-card-actions>
        <v-btn 
          block 
          class="auth-btn" 
          size="large"
          @click="redirectToLogin"
        >
          Return to Sign In
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import '@/assets/css/app-theme.css'
const showTimeout = useState('showSessionTimeout')

const redirectToLogin = () => {
  showTimeout.value = false
  navigateTo('/login')
}
</script>