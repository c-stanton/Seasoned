<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="1200" elevation="1">
      
      <header class="text-center mb-10">
        <v-img 
          src="/images/seasoned-logo.png" 
          width="180" 
          class="mx-auto"
          contain
        ></v-img>
        <p class="collection-title">Your Recipe Collection</p>
      </header>

      <v-row justify="center" class="mb-6">
        <v-col cols="12" md="8" lg="6">
          <v-text-field
            v-model="searchQuery"
            placeholder="Search for 'comfort food' or 'smoothie recipe'"
            variant="outlined"
            class="search-bar"
            hide-details
            clearable
            @click:clear="fetchRecipes"
            :loading="isSearching"
          >
            <template v-slot:prepend-inner>
              <v-icon :color="isSearching ? '#556b2f' : '#2e1e0a'">
                {{ isSearching ? 'mdi-auto-fix' : 'mdi-magnify' }}
              </v-icon>
            </template>
          </v-text-field>
        </v-col>
      </v-row>

      <v-divider class="mb-10 separator"></v-divider>

      <v-row v-if="loading" justify="center" class="py-16">
        <v-col cols="12" class="d-flex flex-column align-center">
          <v-progress-circular indeterminate color="#556b2f" size="64" width="3"></v-progress-circular>
          <p class="brand-subtitle mt-4">Opening Collection...</p>
        </v-col>
      </v-row>

      <v-row v-else-if="recipes?.length">
        <v-col v-for="recipe in sortedRecipes" :key="recipe.id" cols="12" sm="6" md="4">
          <v-card class="gallery-item-card pa-4">
            <v-sheet
              height="200"
              color="#f8f5f0"
              class="rounded-sm mb-4 d-flex align-center justify-center"
              style="border: 1px solid #e8e2d6;"
            >
              <v-img
                v-if="recipe.imageUrl"
                :src="recipe.imageUrl"
                cover
                class="recipe-thumbnail"
              ></v-img>
              <v-icon 
                v-else
                icon="mdi-camera-outline" 
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

    <v-dialog v-model="showDetails" max-width="950" persistent>
      <v-card v-if="selectedRecipe" class="recipe-card pa-8">
        <v-btn
          v-if="!isEditing"
          icon="mdi-close"
          variant="text"
          position="absolute"
          style="top: 10px; right: 10px; z-index: 10;"
          color="#5d4037"
          @click="closeDetails"
        ></v-btn>

        <v-row align="start" class="mb-6">
          <v-col cols="12" md="4" class="d-flex flex-column align-center">
            <v-hover v-slot="{ isHovering, props }">
              <v-card
                v-bind="props"
                width="160"
                height="160"
                :class="[
                  'rounded-lg d-flex align-center justify-center position-relative overflow-hidden',
                  { 
                    'image-drop-zone': isEditing, 
                    'cursor-pointer': isEditing
                  }
                ]"
                :style="{ pointerEvents: isEditing ? 'auto' : 'none' }"
                @click="isEditing ? $refs.fileInput.click() : null" 
                :elevation="isHovering && isEditing ? 4 : 1"
              >
                <v-img
                  v-if="selectedRecipe.imageUrl"
                  :src="selectedRecipe.imageUrl"
                  cover
                  class="rounded-lg fill-height"
                ></v-img>

                <div 
                  v-if="isEditing && (isHovering || !selectedRecipe.imageUrl)" 
                  class="d-flex flex-column align-center justify-center position-absolute"
                  style="background: rgba(226,215,186,0.4); inset: 0;"
                >
                  <v-icon icon="mdi-camera-plus" color="#556b2f" size="large"></v-icon>
                  <span class="brand-subtitle" style="font-size: 0.7rem; color: #556b2f;">Update Photo</span>
                </div>
              </v-card>
            </v-hover>
            <input type="file" ref="fileInput" accept="image/*" style="display: none" @change="handleImageUpload" />
          </v-col>

          <v-col cols="12" md="8">
            <v-text-field
              v-if="isEditing"
              v-model="selectedRecipe.title"
              label="Recipe Title"
              variant="underlined"
              class="recipe-title-edit"
              hide-details
            ></v-text-field>
            <h2 v-else class="recipe-title" style="font-size: 2.5rem;">{{ selectedRecipe.title }}</h2>
          </v-col>
        </v-row>

        <v-divider class="mb-8 separator"></v-divider>

        <v-row class="mt-10" density="compact">
          <v-col cols="12" md="5" class="pe-md-10">
            <h3 class="section-header justify-center mb-6">
              <v-icon icon="mdi-spoon-sugar" class="mr-2" size="small"></v-icon>
              Ingredients
            </h3>
            
            <v-textarea
              v-if="isEditing"
              v-model="selectedRecipe.ingredients"
              variant="outlined"
              auto-grow
              rows="10"
              density="comfortable"
              class="auth-input recipe-textarea"
              bg-color="transparent"
              :persistent-placeholder="true"
            ></v-textarea>
            
            <div v-else class="ingredients-container">
              <div 
                v-for="(ing, index) in (Array.isArray(selectedRecipe.ingredients) ? selectedRecipe.ingredients : selectedRecipe.ingredients?.split('\n') || [])" 
                :key="index"
                class="ingredient-item"
              >
                {{ ing }}
              </div>
            </div>
          </v-col>

          <v-col cols="12" md="7" class="ps-md-10">
            <h3 class="section-header justify-center mb-6">
              <v-icon icon="mdi-pot-steam-outline" class="mr-2" size="small"></v-icon>
              Instructions
            </h3>

            <v-textarea
              v-if="isEditing"
              v-model="selectedRecipe.instructions"
              variant="outlined"
              auto-grow
              rows="10"
              density="comfortable"
              class="auth-input recipe-textarea"
              bg-color="transparent"
              :persistent-placeholder="true"
            ></v-textarea>

            <div v-else
              v-for="(step, index) in (Array.isArray(selectedRecipe.instructions) ? selectedRecipe.instructions : selectedRecipe.instructions?.split('\n') || [])"
              :key="index"
              class="instruction-step mb-8"
            >
              <span class="step-number">{{ index + 1 }}.</span>
              <p class="step-text">{{ step }}</p>
            </div>
          </v-col>
        </v-row>

        <v-card-actions v-if="isEditing" class="justify-center mt-8">
          <v-btn variant="text" @click="saveChanges" class="save-btn px-8">Save Changes</v-btn>
          <v-btn variant="text" @click="isEditing = false" class="cancel-btn px-8">Cancel</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>


<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import '@/assets/css/gallery.css'
import '@/assets/css/app-theme.css'

/*const mockRecipes = [
  {
    id: 1,
    title: "Miso-Glazed Smashed Burger",
    imageUrl: "https://picsum.photos/id/42/800/600",
    ingredients: ["1/2 lb Ground Beef", "1 tbsp White Miso", "Brioche Bun", "Pickled Ginger"],
    instructions: ["Mix miso into beef.", "Smash thin on high heat.", "Sear until crispy."],
    createdAt: "2026-03-18T10:00:00Z"
  },
  {
    id: 2,
    title: "Zesty Yuzu Raspberry Bowl",
    imageUrl: "https://picsum.photos/id/429/800/600",
    ingredients: ["1 cup Oats", "2 tbsp Yuzu Juice", "Fresh Raspberries", "Honey"],
    instructions: ["Soak oats overnight.", "Stir in yuzu.", "Top with berries."],
    createdAt: "2026-03-17T08:30:00Z"
  },
  {
    id: 3,
    title: "Caribbean Curry Chickpeas",
    imageUrl: "https://picsum.photos/id/493/800/600",
    ingredients: ["1 can Chickpeas", "Coconut Milk", "Curry Powder", "Sweet Potato"],
    instructions: ["Sauté potatoes.", "Add chickpeas and spices.", "Simmer in coconut milk."],
    createdAt: "2026-03-16T12:45:00Z"
  },
  {
    id: 4,
    title: "Tallow-Crisped Truffle Fries",
    imageUrl: "https://picsum.photos/id/517/800/600",
    ingredients: ["Russet Potatoes", "Beef Tallow", "Truffle Salt", "Parsley"],
    instructions: ["Double fry in tallow.", "Toss with truffle salt.", "Garnish with parsley."],
    createdAt: "2026-03-15T16:20:00Z"
  },
  {
    id: 5,
    title: "Smoked Gouda & Broccolini Soup",
    imageUrl: "https://picsum.photos/id/488/800/600",
    ingredients: ["Broccolini", "Smoked Gouda", "Heavy Cream", "Vegetable Broth"],
    instructions: ["Simmer broccolini in broth.", "Blend until smooth.", "Melt in cheese."],
    createdAt: "2026-03-14T11:10:00Z"
  },
  {
    id: 6,
    title: "Heirloom Tomato Galette",
    imageUrl: "https://picsum.photos/id/447/800/600",
    ingredients: ["Puff Pastry", "Heirloom Tomatoes", "Ricotta", "Thyme"],
    instructions: ["Spread ricotta on pastry.", "Layer tomatoes.", "Bake at 200°C until golden."],
    createdAt: "2026-03-13T09:00:00Z"
  },
  {
    id: 7,
    title: "Thai Basil Pesto Pasta",
    imageUrl: "https://picsum.photos/id/102/800/600",
    ingredients: ["Linguine", "Thai Basil", "Cashews", "Garlic", "Chili Flakes"],
    instructions: ["Blend basil, cashews, and garlic.", "Toss with hot pasta.", "Add chili for heat."],
    createdAt: "2026-03-12T19:30:00Z"
  },
  {
    id: 8,
    title: "Whipped Feta & Hot Honey Toast",
    imageUrl: "https://picsum.photos/id/311/800/600",
    ingredients: ["Sourdough", "Feta Cheese", "Greek Yogurt", "Hot Honey"],
    instructions: ["Whip feta and yogurt.", "Toast sourdough.", "Spread and drizzle honey."],
    createdAt: "2026-03-11T07:45:00Z"
  }
]
*/
const recipes = ref([])
//const recipes = ref(mockRecipes)
//const loading = ref(false)
const loading = ref(true)
const showDetails = ref(false)
const selectedRecipe = ref(null)
const isEditing = ref(false)
const originalRecipe = ref(null)
const config = useRuntimeConfig()
const searchQuery = ref('')
const isSearching = ref(false)
let debounceTimeout = null

onMounted(async () => {
  await fetchRecipes()
  //loading.value = false
})

const fetchRecipes = async () => {
  try {
    loading.value = true
    const data = await $fetch(`${config.public.apiBase}api/recipe/my-collection`, {
      credentials: 'include' 
    })
    recipes.value = data
  } catch (err) {
    console.error("Failed to load collection:", err)
    if (err.status === 401) navigateTo('/login')
  } finally {
    loading.value = false
  }
}
 
const handleImageUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    selectedRecipe.value.imageUrl = e.target.result;
  };
  reader.readAsDataURL(file);
 
};

const openRecipe = (recipe) => {
  selectedRecipe.value = { ...recipe }
  isEditing.value = false
  showDetails.value = true
}

const editRecipe = (recipe) => {
  const editableRecipe = JSON.parse(JSON.stringify(recipe));
  
  if (Array.isArray(editableRecipe.ingredients)) {
    editableRecipe.ingredients = editableRecipe.ingredients.join('\n');
  }
  if (Array.isArray(editableRecipe.instructions)) {
    editableRecipe.instructions = editableRecipe.instructions.join('\n');
  }

  originalRecipe.value = JSON.parse(JSON.stringify(recipe));
  selectedRecipe.value = editableRecipe;
  isEditing.value = true;
  showDetails.value = true;
}

const closeDetails = () => {
  if (isEditing.value && originalRecipe.value) {
    const index = recipes.value.findIndex(r => r.id === originalRecipe.value.id)
    if (index !== -1) {
      recipes.value[index] = originalRecipe.value
    }
  }
  
  showDetails.value = false
  isEditing.value = false
  originalRecipe.value = null
}

const saveChanges = async () => {
 try {
    const { embedding, ...recipeData } = selectedRecipe.value;
    const payload = { 
      ...selectedRecipe.value,
      createdAt: selectedRecipe.value.createdAt || new Date().toISOString(),
      ingredients: typeof selectedRecipe.value.ingredients === 'string' 
        ? selectedRecipe.value.ingredients.split('\n').filter(i => i.trim()) 
        : selectedRecipe.value.ingredients,
      instructions: typeof selectedRecipe.value.instructions === 'string' 
        ? selectedRecipe.value.instructions.split('\n').filter(i => i.trim()) 
        : selectedRecipe.value.instructions
    };

    await $fetch(`${config.public.apiBase}api/recipe/update/${payload.id}`, {
      method: 'PUT',
      body: payload,
      credentials: 'include'
    });

    const index = recipes.value.findIndex(r => r.id === payload.id);
    if (index !== -1) {
      recipes.value[index] = { ...payload };
    }

    closeDetails();
  } catch (e) {
    console.error("The kitchen ledger could not be updated:", e);
  }
}

const sortedRecipes = computed(() => {
  return [...recipes.value].sort((a, b) => {
    return new Date(b.createdAt) - new Date(a.createdAt)
  })
})

const performSearch = async () => {
  if (!searchQuery.value) {
    await fetchRecipes()
    return
  }

  try {
    isSearching.value = true
    const data = await $fetch(`${config.public.apiBase}api/recipe/search`, {
      params: { query: searchQuery.value },
      credentials: 'include'
    })
    recipes.value = data
  } catch (err) {
    console.error("The Chef couldn't find those flavors:", err)
  } finally {
    isSearching.value = false
  }
}

watch(searchQuery, (newVal) => {
  clearTimeout(debounceTimeout)
  debounceTimeout = setTimeout(() => {
    performSearch()
  }, 600)
})


</script>