<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="1200" elevation="1">
      
      <header class="text-center mb-10">
        <h1 class="brand-title">Your Collection</h1>
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
          <p class="brand-subtitle mt-4">Opening Collection...</p>
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
                :icon="getRecipeIcon(recipe)" 
                size="80" 
                color="#d1c7b7"
              ></v-icon>
            </v-sheet>
            <h3 class="gallery-item-title text-center">{{ recipe.title }}</h3>
            <p class="gallery-item-date text-center">
              Added {{ new Date(recipe.createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' }) }}
            </p>
            
            <v-card-actions class="justify-center">
              <v-card-actions class="justify-center">
                <v-btn 
                  variant="text" 
                  color="#556b2f"
                  class="save-btn"
                  @click="openRecipe(recipe)"
                >
                  Open
                </v-btn>
                <v-btn 
                  variant="text" 
                  color="#8c4a32" 
                  class="cancel-btn"
                  @click="editRecipe(recipe)"
                >
                  Edit
                </v-btn>
              </v-card-actions>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      
      <v-row v-else justify="center" class="py-10 text-center">
        <v-col cols="12">
          <p class="brand-subtitle mb-4">Your collection is empty.</p>
          <v-btn to="/" variant="text" color="#556b2f">Return Home to add some</v-btn>
        </v-col>
      </v-row>

    </v-card>

    <v-dialog v-model="showDetails" max-width="800" persistent>
      <v-card v-if="selectedRecipe" class="recipe-card pa-8">
        <v-btn
          icon="mdi-close"
          variant="text"
          position="absolute"
          style="top: 10px; right: 10px;"
          color="#5d4037"
          @click="closeDetails"
        ></v-btn>

        <header class="text-center mb-6">
          <v-text-field
            v-if="isEditing"
            v-model="selectedRecipe.title"
            variant="underlined"
            class="recipe-title-edit"
          ></v-text-field>
          <h2 v-else class="recipe-title">{{ selectedRecipe.title }}</h2>
        </header>

        <v-divider class="mb-6 separator"></v-divider>

        <v-row justify="center" class="px-md-10">
          <v-col cols="12" md="5" class="d-flex flex-column align-center">
            <div style="width: 100%; max-width: 300px;">
              <h3 class="section-header mb-4">
                <v-icon icon="mdi-basket-outline" class="mr-2" size="small"></v-icon>
                Ingredients
              </h3>
              
              <v-textarea
                v-if="isEditing"
                v-model="selectedRecipe.ingredients"
                variant="outlined"
                auto-grow
                density="comfortable"
                bg-color="rgba(255,255,255,0.3)"
              ></v-textarea>
              
              <v-list v-else class="ingredients-list">
                <v-list-item 
                  v-for="(ing, index) in selectedRecipe.ingredients?.split('\n').filter(i => i.trim())" 
                  :key="index"
                  class="ingredient-item px-0"
                >
                  {{ ing }}
                </v-list-item>
              </v-list>
            </div>
          </v-col>

          <v-col cols="12" md="7">
            <h3 class="section-header mb-4">
              <v-icon icon="mdi-chef-hat" class="mr-2" size="small"></v-icon>
              Instructions
            </h3>

            <v-textarea
              v-if="isEditing"
              v-model="selectedRecipe.instructions"
              variant="outlined"
              auto-grow
              density="comfortable"
              bg-color="rgba(255,255,255,0.3)"
            ></v-textarea>

            <div v-else
              v-for="(step, index) in selectedRecipe.instructions?.split('\n').filter(s => s.trim())" 
              :key="index"
              class="instruction-step mb-4"
            >
              <span class="step-number">{{ index + 1 }}</span>
              <p>{{ step }}</p>
            </div>
          </v-col>
        </v-row>

        <v-card-actions v-if="isEditing" class="justify-center mt-6">
          <v-btn 
            variant="text" 
            @click="saveChanges" 
            class="save-btn px-8"
          >
            Save Changes
          </v-btn>

          <v-btn 
            variant="text" 
            @click="isEditing = false"
            class="cancel-btn px-8"
          >
            Cancel
          </v-btn>
        </v-card-actions>

        <v-divider class="my-6 separator"></v-divider>
        
        <footer class="text-center">
          <p class="brand-subtitle" style="font-size: 0.8rem;">
            Recorded on {{ new Date(selectedRecipe.createdAt).toLocaleDateString() }}
          </p>
        </footer>
      </v-card>
    </v-dialog>
  </v-container>
</template>


<script setup>
import '@/assets/css/gallery.css'
const recipes = ref([])
const loading = ref(true)
const showDetails = ref(false)
const selectedRecipe = ref(null)
const isEditing = ref(false)
const originalRecipe = ref(null)

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
 

const openRecipe = (recipe) => {
  selectedRecipe.value = { ...recipe }
  isEditing.value = false
  showDetails.value = true
}

const editRecipe = (recipe) => {
  selectedRecipe.value = { ...recipe }
  originalRecipe.value = { ...recipe }
  isEditing.value = true
  showDetails.value = true
}

const closeDetails = () => {
  showDetails.value = false
  isEditing.value = false
}

const saveChanges = async () => {
  const token = useCookie('seasoned_token').value

  try {
    await $fetch(`${config.public.apiBase}api/recipe/update/${selectedRecipe.value.id}`, {
      method: 'PUT',
      headers: { 'Authorization': `Bearer ${token}` },
      body: selectedRecipe.value
    })

    await fetchRecipes()
    
    isEditing.value = false
    showDetails.value = false
  } catch (e) {
    console.error("Failed to update recipe:", e)
    alert("Could not save changes. Please try again.")
  }
}

// Mock setup for recipe cards
//const mockData = [
  //  {
   //   id: 1,
   //   title: "Grandma's Secret Bolognese",
   //   createdAt: new Date().toISOString(),
   //   ingredients: "2 lbs Ground Beef\n1 Onion, diced\n3 cloves Garlic\n2 cans Crushed Tomatoes\n1 cup Red Wine",
    //  instructions: "Brown the meat with onions.\nAdd garlic and wine; reduce.\nSimmer with tomatoes for 3 hours."
   // },
   // {
      //id: 2,
      //title: "Rustic Sourdough",
      //createdAt: new Date().toISOString(),
      //ingredients: "500g Flour\n350g Water\n100g Starter\n10g Salt",
      //instructions: "Mix and autolyse for 1 hour.\nPerform 4 sets of stretch and folds.\nCold ferment for 12 hours.\nBake at 450°F in a dutch oven."
    //}
  //]
  //recipes.value = mockData
  //loading.value = false
//}

//const saveChanges = async () => {

  //const index = recipes.value.findIndex(r => r.id === selectedRecipe.value.id)
  //if (index !== -1) {
    //recipes.value[index] = { ...selectedRecipe.value }
  //}

  //isEditing.value = false
  //showDetails.value = false
//}

const getRecipeIcon = (title) => {
  const t = title.toLowerCase()
  if (recipe.icon) return recipe.icon
  if (t.includes('cake') || t.includes('cookie') || t.includes('dessert')) return 'mdi-cookie'
  if (t.includes('soup') || t.includes('stew')) return 'mdi-bowl-mix'
  if (t.includes('drink') || t.includes('cocktail')) return 'mdi-glass-cocktail'
  return 'mdi-silverware-fork-knife'
}
</script>