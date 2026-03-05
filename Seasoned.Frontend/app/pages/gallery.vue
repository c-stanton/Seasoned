<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="1200" elevation="1">
      
      <header class="text-center mb-10">
        <h1 class="brand-title">The Collection</h1>
        <p class="brand-subtitle">Hand-Picked & Seasoned</p>
      </header>

      <v-divider class="mb-10 separator"></v-divider>

      <v-btn to="/" variant="text" class="back-link mb-8" color="#6d5e4a">
        <v-icon icon="mdi-chevron-left" class="mr-1"></v-icon>
        Return to Scanner
      </v-btn>

      <v-row>
        <v-col v-for="n in 6" :key="n" cols="12" sm="6" md="4">
          <v-card class="gallery-item-card pa-4" elevation="2">
            <v-img
              src="https://images.unsplash.com/photo-1546069901-ba9599a7e63c"
              height="200"
              cover
              class="rounded-sm mb-4 recipe-thumbnail"
            >
              <template v-slot:placeholder>
                <v-row class="fill-height ma-0" align="center" justify="center">
                  <v-progress-circular indeterminate color="#556b2f"></v-progress-circular>
                </v-row>
              </template>
            </v-img>

            <h3 class="gallery-item-title text-center">Grandma's Stew</h3>
            <p class="gallery-item-date text-center">Added March 2026</p>
            
            <v-card-actions class="justify-center">
              <v-btn variant="text" class="view-recipe-btn" color="#556b2f">
                Open Recipe
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-card>
  </v-container>
</template>