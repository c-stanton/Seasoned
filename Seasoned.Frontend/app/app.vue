<template>
  <v-app :class="['recipe-bg', { 'landing-page': $route.path === '/' }]">
    
    <v-app-bar color="transparent" flat elevation="0" class="px-4" height="70">
      <v-btn to="/" variant="text" class="nav-home-btn">Seasoned</v-btn>
      <v-spacer></v-spacer>

      <div class="nav-links d-flex align-center">
        <v-menu v-if="isLoggedIn" transition="slide-y-transition">
          <template v-slot:activator="{ props }">
            <v-btn 
              v-bind="props" 
              variant="text" 
              class="nav-auth-btn px-4 d-flex align-center"
            >
              <v-icon icon="mdi-pot-steam" size="small" class="mr-2"></v-icon>
              <span class="menu-label-text">Menu</span>
            </v-btn>
          </template>

          <v-list class="recipe-card pa-2 mt-2" elevation="4" border>
            <v-list-item to="/uploader" prepend-icon="mdi-camera-outline">
              <v-list-item-title class="menu-text">Recipe Uploader </v-list-item-title>
            </v-list-item>
            <v-list-item to="/chat" prepend-icon="mdi-chef-hat">
              <v-list-item-title class="menu-text">Chef Consultation</v-list-item-title>
            </v-list-item>
            <v-list-item to="/gallery" prepend-icon="mdi-book-open-variant" class="rounded">
              <v-list-item-title class="menu-text">My Collection</v-list-item-title>
            </v-list-item>

            <v-divider class="ma-0"></v-divider>

            <v-list-item @click="logout" prepend-icon="mdi-logout" color="error" class="rounded mt-0">
              <v-list-item-title class="menu-text">Sign Out</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-btn v-else to="/login" variant="text" class="nav-auth-btn">Sign In</v-btn>
      </div>
    </v-app-bar>

    <v-main>
      <NuxtPage />
    </v-main>
  </v-app>
</template>

<script setup>
import { onMounted } from 'vue'
import '@/assets/css/app-theme.css'
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