<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="1200" elevation="1">
      
      <header class="text-center mb-10">
        <h1 class="brand-title">The Collection</h1>
        <p class="brand-subtitle">Hand-Picked & Seasoned</p>
      </header>

      <v-divider class="mb-10 separator"></v-divider>

      <v-btn 
        to="/" 
        class="back-to-home-btn mb-10" 
        size="large" 
        elevation="0"
        block
      >
        <v-icon icon="mdi-arrow-left" class="mr-2"></v-icon>
        Back to Recipe Upload
      </v-btn>

      <v-row v-if="recipes?.length">
        <v-col v-for="recipe in recipes" :key="recipe.id" cols="12" sm="6" md="4">
          <v-card class="gallery-item-card pa-4">
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

            <h3 class="gallery-item-title text-center">{{ recipe.title }}</h3>
            <p class="gallery-item-date text-center">
              Added {{ new Date(recipe.createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' }) }}
            </p>
            
            <v-card-actions class="justify-center">
              <v-btn variant="text" class="view-recipe-btn" color="#556b2f">
                Open Recipe
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      
      <v-row v-else justify="center" class="py-10 text-center">
        <v-col cols="12">
          <p class="brand-subtitle mb-4">Your collection is empty.</p>
        </v-col>
      </v-row>

    </v-card>
  </v-container>
</template>

<script setup>
import '@/assets/css/gallery.css'
//const config = useRuntimeConfig();
//const { data: recipes } = await useFetch(`${config.public.apiBase}api/recipe`);
const recipes = ref([
  {
    id: 1,
    title: 'Salmon Salad',
    createdAt: '2026-03-05T12:00:00Z',
    description: 'A fresh herb-crusted salmon over greens.'
  },
  {
    id: 2,
    title: 'Power Bowl',
    createdAt: '2026-03-04T10:00:00Z',
    description: 'Rich and earthy pesto with toasted walnuts.'
  },
  {
    id: 3,
    title: 'Protein Slam',
    createdAt: '2026-03-01T08:00:00Z',
    description: 'Crispy crust with a light, airy center.'
  }
]);
</script>