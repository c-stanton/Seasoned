<template>
  <v-app class="recipe-bg">
    <v-app-bar 
      color="transparent"
      flat
      elevation="0"
      class="px-4"
      height="70"
    >
      <v-btn
        to="/"
        variant="text"
        class="nav-home-btn ml-4"
        elevation="0"
      >
        Seasoned
      </v-btn>

      <v-spacer></v-spacer>

      <div class="nav-links d-flex align-center">
        <v-btn 
          to="/gallery"
          variant="text" 
          class="nav-auth-btn ml-4" 
          elevation="0"
        >
          Collection
        </v-btn>

        <v-btn 
          v-if="!isLoggedIn" 
          to="/login" 
          variant="text"
          class="nav-auth-btn ml-4" 
          elevation="0"
        >
          Sign In
        </v-btn>
        
        <v-btn 
          v-else 
          @click="logout" 
          variant="text"
          class="nav-auth-btn ml-4" 
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
const isLoggedIn = useState('isLoggedIn', () => false)
const tokenCookie = useCookie('seasoned_token')

onMounted(() => {
  if (tokenCookie.value) {
    isLoggedIn.value = true
  }
})

const logout = () => {
  tokenCookie.value = null
  isLoggedIn.value = false
  
  if (import.meta.client) {
    localStorage.removeItem('token')
  }
  
  navigateTo('/login')
}
</script>