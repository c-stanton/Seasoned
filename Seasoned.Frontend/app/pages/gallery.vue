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
                  'rounded-lg d-flex align-center justify-center cursor-pointer position-relative overflow-hidden',
                  { 'image-drop-zone': isEditing }
                ]"
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

        <v-row class="px-md-4">
          <v-col cols="12" md="5">
            <h3 class="section-header mb-4">
              <v-icon icon="mdi-basket-outline" class="mr-2" size="small"></v-icon>
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
            
            <v-list v-else class="ingredients-list bg-transparent">
              <v-list-item 
                v-for="(ing, index) in (Array.isArray(selectedRecipe.ingredients) ? selectedRecipe.ingredients : selectedRecipe.ingredients?.split('\n') || [])" 
                :key="index"
                class="ingredient-item px-0"
              >
                • {{ ing }}
              </v-list-item>
            </v-list>
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
              rows="10"
              density="comfortable"
              class="auth-input recipe-textarea"
              bg-color="transparent"
              :persistent-placeholder="true"
            ></v-textarea>

            <div v-else
              v-for="(step, index) in (Array.isArray(selectedRecipe.instructions) ? selectedRecipe.instructions : selectedRecipe.instructions?.split('\n') || [])"
              :key="index"
              class="instruction-step mb-4"
            >
              <span class="step-number">{{ index + 1 }}</span>
              <p>{{ step }}</p>
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
import { ref, onMounted, computed } from 'vue'
import '@/assets/css/gallery.css'
import '@/assets/css/app-theme.css'

const recipes = ref([])
const loading = ref(true)
const showDetails = ref(false)
const selectedRecipe = ref(null)
const isEditing = ref(false)
const originalRecipe = ref(null)
const config = useRuntimeConfig()

onMounted(async () => {
  await fetchRecipes()
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
</script>