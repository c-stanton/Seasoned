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

      <v-row v-if="loading" justify="center" class="py-16">
        <v-col cols="12" class="d-flex flex-column align-center">
          <v-progress-circular indeterminate color="#556b2f" size="64" width="3"></v-progress-circular>
          <p class="brand-subtitle mt-4">Opening the Ledger...</p>
        </v-col>
      </v-row>

      <v-row v-else-if="recipes?.length">
        <v-col v-for="recipe in recipes" :key="recipe.id" cols="12" sm="6" md="4">
          <v-card class="gallery-item-card pa-4">
            <v-sheet
              height="200"
              color="#f8f5f0"
              class="rounded-sm mb-4 d-flex align-center justify-center"
              style="border: 1px solid #e8e2d6;"
            >
              <v-icon 
                :icon="getRecipeIcon(recipe.title)" 
                size="80" 
                color="#d1c7b7"
              ></v-icon>
            </v-sheet>
            <h3 class="gallery-item-title text-center">{{ recipe.title }}</h3>
            <p class="gallery-item-date text-center">
              Added {{ new Date(recipe.createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' }) }}
            </p>
            
            <v-card-actions class="justify-center">
              <v-btn 
                variant="text" 
                class="view-recipe-btn" 
                color="#556b2f"
                @click="openRecipe(recipe)"
              >
                Open Recipe
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      
      <v-row v-else justify="center" class="py-10 text-center">
        <v-col cols="12">
          <p class="brand-subtitle mb-4">Your collection is empty.</p>
          <v-btn to="/" variant="text" color="#556b2f">Return to kitchen to add some</v-btn>
        </v-col>
      </v-row>

    </v-card>
  </v-container>
</template>


<script setup>
import '@/assets/css/gallery.css'
const config = useRuntimeConfig()
const recipes = ref([])
const loading = ref(true)
const showDetails = ref(false)
const selectedRecipe = ref(null)

onMounted(async () => {
  await fetchRecipes()
})

const fetchRecipes = async () => {
  const token = useCookie('seasoned_token').value 
                || (import.meta.client ? localStorage.getItem('token') : null)

  if (!token) {
    return navigateTo('/login')
  }

  try {
    loading.value = true
    const data = await $fetch(`${config.public.apiBase}api/recipe/my-collection`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })
    recipes.value = data
  } catch (err) {
    console.error("Failed to load collection:", err)
    if (err.status === 401) navigateTo('/login')
  } finally {
    loading.value = false
  }
}
</script>