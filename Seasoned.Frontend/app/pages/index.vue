<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="950" elevation="1">
      
      <header class="text-center mb-10">
        <h1 class="brand-title">Seasoned</h1>
        <p class="brand-subtitle">A Recipe Collection</p>
      </header>
      
      <v-divider class="mb-10 separator"></v-divider>

      <v-row justify="center" class="mb-12">
        <v-col cols="12" md="8" class="d-flex flex-column align-center">
          <div 
            class="drop-zone mb-4"
            :class="{ 'drop-zone--active': isDragging }"
            @dragover.prevent="isDragging = true"
            @dragleave.prevent="isDragging = false"
            @drop.prevent="handleDrop"
            @click="$refs.fileInput.click()"
          >
            <v-icon icon="mdi-cloud-upload-outline" size="large" class="mb-2"></v-icon>
            <p v-if="!files || files.length === 0" class="drop-text">
              Drag your recipe photo here or <strong>click to browse</strong>
            </p>
            <p v-else class="drop-text selected-text">
              {{ Array.isArray(files) ? files[0].name : files.name }}
            </p>

            <v-file-input
              ref="fileInput"
              v-model="files"
              accept="image/*"
              class="d-none"
              hide-details
            ></v-file-input>
          </div>

          <div class="d-flex w-100 mt-4 align-center">
            <v-btn 
              class="analyze-btn flex-grow-1 mr-2"
              size="large"
              elevation="0"
              :loading="loading"
              :disabled="!files || files.length === 0"
              @click="uploadImage"
            >
              <v-icon icon="mdi-pot-steam" class="mr-2"></v-icon>
              Analyze Recipe
            </v-btn>

            <v-btn
              class="clear-btn-solid"
              variant="flat"
              size="large"
              elevation="0"
              @click="clearAll"
            >
              <v-icon icon="mdi-refresh"></v-icon>
            </v-btn>
          </div>

          <v-btn
            class="gallery-btn w-100 mt-4"
            size="large"
            elevation="0"
            @click="handleViewCollection"
          >
            <v-icon icon="mdi-book-open-variant" class="mr-2"></v-icon>
            View Collection
          </v-btn>
        </v-col>
      </v-row>

      <transition name="fade">
        <div v-if="recipe" class="recipe-content">
          <h2 class="recipe-title text-center mb-4">{{ recipe.title }}</h2>
          <p class="recipe-description text-center mb-12 text-italic">{{ recipe.description }}</p>

          <v-row>
            <v-col cols="12" md="5">
              <div class="section-header mb-6 px-2">
                <v-icon icon="mdi-spoon-sugar" class="mr-2" size="small"></v-icon>
                <span>Ingredients</span>
              </div>
              <v-list class="ingredients-list">
                <v-list-item v-for="(item, i) in recipe.ingredients" :key="i" class="ingredient-item">
                  {{ item }}
                </v-list-item>
              </v-list>
            </v-col>

            <v-col cols="12" md="7">
              <div class="section-header mb-6 px-2">
                <v-icon icon="mdi-pot-steam-outline" class="mr-2" size="small"></v-icon>
                <span>Instructions</span>
              </div>
              <div v-for="(step, i) in recipe.instructions" :key="i" class="instruction-step mb-8">
                <span class="step-number">{{ i + 1 }}.</span>
                <p class="step-text">{{ step }}</p>
              </div>
            </v-col>
          </v-row>

          <v-row justify="center" class="mt-12 pb-10">
            <v-btn
              v-if="!hasSaved"
              class="save-recipe-btn px-12"
              size="large"
              elevation="0"
              :loading="saving"
              @click="saveToCollection"
            >
              <v-icon icon="mdi-content-save-check-outline" class="mr-2"></v-icon>
              Save to Collection
            </v-btn>
          </v-row>

        </div>
      </transition>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import '@/assets/css/app-theme.css'

const router = useRouter()
const config = useRuntimeConfig()
const files = ref([])
const loading = ref(false)
const recipe = ref(null)
const isDragging = ref(false)
const saving = ref(false)
const hasSaved = ref(false)

const isAuthenticated = () => {
  if (import.meta.client) {
    return !!localStorage.getItem('token');
  }
  return false;
}

const handleViewCollection = () => {
  if (isAuthenticated()) {
    router.push('/gallery')
  } else {
    router.push('/login')
  }
}

const handleDrop = (e) => {
  isDragging.value = false
  const droppedFiles = e.dataTransfer.files
  if (droppedFiles.length > 0) {
    files.value = droppedFiles[0]
  }
}

const uploadImage = async () => {
  const fileToUpload = Array.isArray(files.value) ? files.value[0] : files.value;
  if (!fileToUpload) return;

  loading.value = true;
  recipe.value = null;
  hasSaved.value = false;

  const formData = new FormData();
  formData.append('image', fileToUpload);

  try {
    const response = await $fetch(`${config.public.apiBase}api/recipe/upload`, {
      method: 'POST',
      body: formData
    });
    recipe.value = response;
  } catch (error) {
    console.error("Error:", error);
  } finally {
    loading.value = false;
  }
}

const saveToCollection = async () => {
  if (!recipe.value || hasSaved.value) return;

  // 1. Get the token (same logic as gallery)
  const token = useCookie('seasoned_token').value 
                || (import.meta.client ? localStorage.getItem('token') : null)

  if (!token) {
    alert("Please sign in to save recipes.")
    return navigateTo('/login')
  }

  saving.value = true;

  try {
    await $fetch(`${config.public.apiBase}api/recipe/save`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: recipe.value
    });
    hasSaved.value = true;
  } catch (error) {
    console.error("Save failed:", error);
    alert("Could not save recipe. Your session might have expired.")
  } finally {
    saving.value = false;
  }
}

const clearAll = () => {
  files.value = []
  recipe.value = null
  hasSaved.value = false
  loading.value = false
  saving.value = false
}
</script>