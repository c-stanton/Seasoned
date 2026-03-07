<template>
  <v-app class="recipe-bg">
    <v-app-bar 
      color="transparent"
      flat
      elevation="0"
      class="px-4"
      height="70"
    >
      <v-app-bar-title 
        class="nav-brand" 
        style="cursor: pointer; user-select: none;" 
        @click="navigateTo('/')"
      >
        Seasoned
      </v-app-bar-title>

      <v-spacer></v-spacer>

      <div class="nav-links d-flex align-center">
        <v-btn 
          to="/gallery" 
          class="nav-auth-btn ml-4" 
          variant="outlined"
          elevation="0"
        >
          Collection
        </v-btn>

        <v-btn 
          v-if="!token" 
          to="/login" 
          class="nav-auth-btn ml-4" 
          variant="outlined"
          elevation="0"
        >
          Sign In
        </v-btn>
        
        <v-btn 
          v-else 
          @click="logout" 
          class="nav-auth-btn ml-4" 
          variant="outlined"
          elevation="0"
        >
          Logout
        </v-btn>
      </div>
    </v-app-bar>

    <v-main>
      <NuxtPage />
    </v-main>
  </v-app>
</template>

<script setup>
import '@/assets/css/app-theme.css'

const token = useCookie('seasoned_token')

const logout = () => {
  token.value = null
  
  if (import.meta.client) {
    localStorage.removeItem('token')
  }
  
  navigateTo('/login')
}
</script>