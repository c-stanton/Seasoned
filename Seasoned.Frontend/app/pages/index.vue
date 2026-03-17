<template>
  <v-container fluid class="pa-0 landing-wrapper">
    <v-row no-gutters justify="center" align="start" class="pt-6">
      <v-col cols="12" class="text-center px-4">
        
        <v-card class="recipe-card pa-8 mx-auto mt-2" max-width="900">
          
          <header class="mb-10">
            <div class="brand-icon-container mb-2">
              <v-img 
                src="/images/seasoned-logo.png" 
                width="180" 
                class="mx-auto"
                contain
              ></v-img>
            </div>
            <h1 class="brand-title mt-0 mb-1">Seasoned</h1>
            <p class="brand-subtitle mb-8">A Recipe Generator and Collection Tool</p>
          </header>

          <v-divider class="mb-10 separator"></v-divider>

          <v-row class="mb-12 px-6" justify="center">
            <v-col cols="12" md="4" class="text-center d-flex flex-column align-center">
              <v-icon icon="mdi-folder-text" size="large" style="color: #2e1e0a" class="mb-3"></v-icon>
              <h3 class="section-header justify-center mb-3">Scan Archives</h3>
              <p class="feature-text text-body-2">
                Turn handwritten cards into searchable digital text instantly.
              </p>
              <v-spacer></v-spacer>
              <v-btn v-if="isLoggedIn" to="/uploader"  class="mt-12 column-btn">
                Got to Uploader
              </v-btn>
            </v-col>
            <v-col cols="12" md="4" class="text-center d-flex flex-column align-center">
              <v-icon icon="mdi-chef-hat" size="large" style="color: #2e1e0a" class="mb-3"></v-icon>
              <h3 class="section-header justify-center mb-3">Consult the Chef</h3>
              <p class="feature-text text-body-2">
                Chat with an AI chef to scale ingredients, find substitutes, or get inspiration.
              </p>
              <v-spacer></v-spacer>
              <v-btn v-if="isLoggedIn" to="/chat"  class="mt-12 column-btn">
                Talk to Chef
              </v-btn>
            </v-col>
            <v-col cols="12" md="4" class="text-center d-flex flex-column align-center">
              <v-icon icon="mdi-book-open-variant" size="large" style="color: #2e1e0a" class="mb-3"></v-icon>
              <h3 class="section-header justify-center mb-3">Preserve History</h3>
              <p class="feature-text text-body-2">
                Build a private collection that keeps your family traditions alive and organized.
              </p>
              <v-spacer></v-spacer>
              <v-btn v-if="isLoggedIn" to="/gallery"  class="mt-12 column-btn">
                View Collection
              </v-btn>
            </v-col>
          </v-row>
          <div v-if="!isLoggedIn" class="d-flex flex-column align-center">
            <v-btn to="/login" class="column-btn" size="x-large">
              Get Started
            </v-btn>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import '@/assets/css/app-theme.css'

const isLoggedIn = useState('isLoggedIn', () => false)

</script>