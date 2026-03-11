<template>
  <v-container class="fill-height">
    <v-card class="recipe-card auth-card pa-10 mx-auto" elevation="10" max-width="500">
      
      <v-fade-transition mode="out-in">
        <div :key="isLogin">
          <header class="text-center mb-8">
            <h1 class="auth-title">{{ isLogin ? 'Sign In' : 'Join Us' }}</h1>
          </header>

          <v-form @submit.prevent="handleAuth">
            <v-text-field
              autofocus
              v-model="email"
              placeholder="Email"
              bg-color="#5d4037"
              base-color="#f8f1e0"
              class="auth-input mb-4"
              variant="outlined"
              hide-details
              prepend-inner-icon="mdi-email-outline"
            ></v-text-field>

            <v-text-field
              v-model="password"
              placeholder="Password"
              bg-color="#5d4037"
              base-color="#f8f1e0"
              type="password"
              class="auth-input mb-6"
              variant="outlined"
              hide-details
              prepend-inner-icon="mdi-lock-outline"
            ></v-text-field>

            <v-btn
              block
              class="analyze-btn mb-4"
              size=""
              elevation="0"
              type="submit"
            >
              {{ isLogin ? 'Login' : 'Create Account' }}
            </v-btn>

            <div class="text-center">
              <span class="auth-toggle-btn" @click="isLogin = !isLogin" style="cursor: pointer;">
                {{ isLogin ? "New here? Register an account" : "Already a member? Sign in" }}
              </span>
            </div>
          </v-form>
        </div>
      </v-fade-transition>

      <v-divider class="my-6 separator"></v-divider>

      <v-btn to="/" variant="text" color="#6d5e4a" block class="view-recipe-btn">
        <v-icon icon="mdi-chevron-left" class="mr-1"></v-icon>
        Return to Home
      </v-btn>
    </v-card>
  </v-container>
</template>

<script setup>
const isLogin = ref(true)
const email = ref('')
const password = ref('')
const config = useRuntimeConfig()

const handleAuth = async () => {
  const endpoint = isLogin.value ? 'api/auth/login' : 'api/auth/register'
  
  const url = isLogin.value 
    ? `${config.public.apiBase}${endpoint}?useCookies=true` 
    : `${config.public.apiBase}${endpoint}`

  try {
    const response = await $fetch(url, {
      method: 'POST',
      body: {
        email: email.value,
        password: password.value
      }
    })

    if (isLogin.value) {
      navigateTo('/gallery')
    
    }
  } catch (err) {
    alert("Authentication failed. Check your credentials.")
  }
}
</script>