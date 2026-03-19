<template>
  <v-app :class="['recipe-bg', { 'landing-page': $route.path === '/' }]">
    
    <v-app-bar color="transparent" flat elevation="0" class="px-4" height="70">
      <v-btn to="/" variant="text" class="nav-home-btn">Seasoned</v-btn>
      <v-spacer></v-spacer>

      <div class="nav-links d-flex align-center">
        <v-menu v-if="isLoggedIn" transition="slide-y-transition" offset="8">
          <template v-slot:activator="{ props }">
            <v-btn v-bind="props" variant="text" class="nav-auth-btn px-4">
              <v-icon icon="mdi-pot-steam" size="small" class="mr-2"></v-icon>
              <span>Menu</span>
            </v-btn>
          </template>

          <v-list class="menu-card mt-2" min-width="220">
            <v-list-item to="/uploader" prepend-icon="mdi-camera-outline">
              <v-list-item-title class="menu-text">Recipe Uploader</v-list-item-title>
            </v-list-item>

            <v-list-item to="/chat" prepend-icon="mdi-chef-hat">
              <v-list-item-title class="menu-text">Chef Consultation</v-list-item-title>
            </v-list-item>

            <v-list-item to="/gallery" prepend-icon="mdi-book-open-variant">
              <v-list-item-title class="menu-text">My Collection</v-list-item-title>
            </v-list-item>

            <v-divider class="ma-0" color="#dccca7"></v-divider>

            <v-list-item @click="logout" prepend-icon="mdi-logout">
              <v-list-item-title class="menu-text" style="color: #8c4a32;">Sign Out</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-btn v-else to="/login" variant="text" class="nav-auth-btn">Sign In</v-btn>
      </div>
    </v-app-bar>

    <v-main>
      <NuxtPage />
      <SessionTimeout />
    </v-main>
  </v-app>
</template>

<script setup>
import { onMounted } from 'vue'
import '@/assets/css/app-theme.css'
import SessionTimeout from './components/SessionTimeout.vue'
const authCookie = useCookie('.AspNetCore.Identity.Application')
const isLoggedIn = useState('isLoggedIn', () => false)

onMounted(() => {
  if (authCookie.value) isLoggedIn.value = true
})

const logout = () => {
  authCookie.value = null
  isLoggedIn.value = false
  if (import.meta.client) localStorage.removeItem('token')
  navigateTo('/login')
}
</script>