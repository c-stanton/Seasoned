<template>
  <v-container class="fill-height">
    <v-card theme="light" class="recipe-card auth-card pa-10 mx-auto" elevation="10" max-width="500">
      
      <v-fade-transition mode="out-in">
        <div :key="isLogin">
          <header class="text-center mb-10">
            <div class="brand-icon-container mb-1">
              <v-img 
                src="/images/seasoned-logo.png" 
                width="180" 
                class="mx-auto"
                contain
              ></v-img>
            </div>
            <h1 class="auth-title">{{ isLogin ? 'Sign In' : 'Join Us' }}</h1>
          </header>

          <v-form @submit.prevent="handleAuth">
            <v-expand-transition>
              <div v-if="errorMessage" class="mb-6"> <div 
                  :class="[
                    'auth-message', 
                    errorMessage.includes('created') ? 'auth-success' : 'auth-error'
                  ]"
                >
                  <v-icon 
                    :icon="errorMessage.includes('created') ? 'mdi-check-circle-outline' : 'mdi-alert-circle-outline'" 
                    size="small" 
                    class="mr-2"
                  ></v-icon>
                  <span>{{ errorMessage }}</span>
                </div>
              </div>
            </v-expand-transition>
            
            <v-text-field
              autofocus
              v-model="email"
              label="Email Address"
              class="mb-0 auth-input"
              color="#8c4a32"
              variant="outlined"
              prepend-inner-icon="mdi-email-outline"
              @input="errorMessage = ''"
              :style="{ 
                caretColor: '#2e1e0a !important',
                fontFamily: 'Libre Baskerville, serif !important'
              }"
            ></v-text-field>

            <v-text-field
              v-model="password"
              label="Password"
              type="password"
              class="mb-6 auth-input"
              variant="outlined"
              color="#8c4a32"
              hide-details
              prepend-inner-icon="mdi-lock-outline"
              @input="errorMessage = ''"
              :style="{ 
                caretColor: '#2e1e0a !important',
                fontFamily: 'Libre Baskerville, serif !important'
              }"
            ></v-text-field>
            
            <v-expand-transition>
              <v-text-field
                v-if="!isLogin"
                v-model="confirmPassword"
                label="Confirm Password"
                type="password"
                class="mb-6 auth-input"
                variant="outlined"
                color="#8c4a32"
                hide-details
                prepend-inner-icon="mdi-lock-check-outline"
                @input="errorMessage = ''"
                :style="{ 
                caretColor: '#2e1e0a !important',
                fontFamily: 'Libre Baskerville, serif !important'
              }"
              ></v-text-field>
            </v-expand-transition>

            <v-btn
              block
              class="auth-btn mb-4"
              size="large"
              type="submit"
              :loading="authLoading"
              :disabled="authLoading"
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
import { ref } from 'vue'
import '@/assets/css/login.css'
import '@/assets/css/app-theme.css'

const isLogin = ref(true)
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')
const authLoading = ref(false)
const config = useRuntimeConfig()

const isLoggedIn = useState('isLoggedIn', () => false)

const toggleMode = () => {
  isLogin.value = !isLogin.value
  errorMessage.value = ''
  confirmPassword.value = ''
}

const handleAuth = async () => {
  errorMessage.value = ''
  if (!isLogin.value && password.value !== confirmPassword.value) {
    errorMessage.value = "Passwords do not match."
    return
  }

  authLoading.value = true
  const endpoint = isLogin.value ? 'api/auth/login' : 'api/auth/register'
  
  const url = isLogin.value 
    ? `${config.public.apiBase}${endpoint}?useCookies=true&useSessionCookies=false` 
    : `${config.public.apiBase}${endpoint}`

  try {
    const response = await $fetch(url, {
      method: 'POST',
      body: {
        email: email.value,
        password: password.value
      },
      credentials: 'include' 
    })

    if (isLogin.value) {
      isLoggedIn.value = true
      navigateTo('/')
    } else {
      isLogin.value = true
      authLoading.value = false
      errorMessage.value = "Account created! Please sign in."
      password.value = ''
      confirmPassword.value = ''
    }
  } catch (err) {
    authLoading.value = false
    if (err.status === 401) {
      errorMessage.value = "Invalid email or password. Please try again."
    } else if (err.status === 400) {
      errorMessage.value = "Registration failed. Check your password length."
    } else if (err.status === 404) {
      errorMessage.value = "Account not found. Would you like to register?"
    } else {
      errorMessage.value = "Something went wrong."
    }
    console.error('Auth error:', err)
  }
}
</script>