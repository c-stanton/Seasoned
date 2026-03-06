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

onMounted(async () => {
  await fetchRecipes()
})

const fetchRecipes = async () => {
  // Get the token we saved during login
  const token = useCookie('seasoned_token').value 
                || (import.meta.client ? localStorage.getItem('token') : null)

  if (!token) {
    // If no token, kick them back to login
    return navigateTo('/login')
  }

  try {
    loading.value = true
    // You'll need to add this GET endpoint to your RecipeController
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