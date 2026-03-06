<template>
  <v-container class="fill-height">
    <v-card class="recipe-card auth-card pa-10 mx-auto" elevation="10">
      
      <v-fade-transition mode="out-in">
        <div :key="isLogin">
          <header class="text-center mb-8">
            <h1 class="auth-title">{{ isLogin ? 'Sign In' : 'Join Us' }}</h1>
            <p class="brand-subtitle">The Seasoned Ledger</p>
          </header>

          <v-form @submit.prevent="handleAuth">
            <v-text-field
              v-if="!isLogin"
              label="Name"
              placeholder="Your name"
              class="custom-input auth-input mb-4"
              variant="flat"
              prepend-inner-icon="mdi-account-outline"
            ></v-text-field>

            <v-text-field
              v-model="email"
              label="Email"
              placeholder="email@example.com"
              class="custom-input auth-input mb-4"
              variant="flat"
              prepend-inner-icon="mdi-email-outline"
            ></v-text-field>

            <v-text-field
              v-model="password"
              label="Password"
              type="password"
              placeholder="••••••••"
              class="custom-input auth-input mb-6"
              variant="flat"
              prepend-inner-icon="mdi-lock-outline"
            ></v-text-field>

            <v-btn
              block
              class="analyze-btn mb-4"
              size="large"
              elevation="0"
              type="submit"
            >
              {{ isLogin ? 'Open Ledger' : 'Create Account' }}
            </v-btn>

            <div class="text-center">
              <span class="auth-toggle-btn" @click="isLogin = !isLogin">
                {{ isLogin ? "New here? Register an account" : "Already a member? Sign in" }}
              </span>
            </div>
          </v-form>
        </div>
      </v-fade-transition>

      <v-divider class="my-6 separator"></v-divider>

      <v-btn to="/" variant="text" color="#6d5e4a" block class="view-recipe-btn">
        <v-icon icon="mdi-chevron-left" class="mr-1"></v-icon>
        Return to Kitchen
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
  
  const url = `${config.public.apiBase}${endpoint}?useCookies=false`

  try {
    const response = await $fetch(url, {
      method: 'POST',
      body: {
        email: email.value,
        userName: email.value,
        password: password.value
      }
    })

    if (isLogin.value && response.accessToken) {
       navigateTo('/gallery')
    }
  } catch (err) {
    alert("Authentication failed. Check your credentials.")
  }
}
</script>