<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="950" elevation="1">
      <header class="text-center mb-10">
        <v-img 
            src="/images/seasoned-logo.png" 
            width="180" 
            class="mx-auto"
            contain
          >
        </v-img>
        <p class="chat-title">Recipe Uploader</p>
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
            <v-icon icon="mdi-cloud-upload-outline" size="large" class="mb-2" color="#5d4a36"></v-icon>
            <p v-if="!files || files.length === 0" class="drop-text">
              Drag your recipe photo here or <strong>click to browse</strong>
            </p>
            <p v-else class="drop-text selected-text">
              {{ Array.isArray(files) ? files[0]?.name : files?.name }}
            </p>

            <v-file-input
              ref="fileInput"
              v-model="files"
              accept="image/*"
              class="d-none"
              hide-details
            ></v-file-input>
          </div>


          <div class="d-flex w-100 mt-4">
            <v-btn class="analyze-btn flex-grow-1 mr-2" size="large" :loading="loading" :disabled="!files" @click="uploadImage">
              <v-icon icon="mdi-pot-steam" class="mr-2"></v-icon>Analyze Recipe
            </v-btn>
            <v-btn class="clear-btn-solid" size="large" @click="clearAll"><v-icon icon="mdi-refresh"></v-icon></v-btn>
          </div>
        </v-col>
      </v-row>

      <RecipeDisplay 
        :recipe="recipe" 
        :is-saving="saving" 
        :has-saved="hasSaved"
        @save="saveToCollection" 
      />
    </v-card>
  </v-container>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import '@/assets/css/app-theme.css'

const router = useRouter()
const config = useRuntimeConfig()
const files = ref(null)
const loading = ref(false)
const recipe = ref(null)
const isDragging = ref(false)
const saving = ref(false)
const hasSaved = ref(false)

onMounted(() => {
  const savedRecipe = localStorage.getItem('pending_recipe')
  if (savedRecipe) {
    recipe.value = JSON.parse(savedRecipe)
    localStorage.removeItem('pending_recipe')
  }
})

const isAuthenticated = async () => {
  try {
    await $fetch('/api/auth/manage/info', { credentials: 'include' })
    return true
  } catch {
    return false
  }
}

const handleDrop = (e) => {
  isDragging.value = false
  const droppedFiles = e.dataTransfer.files
  if (droppedFiles.length > 0) {
    files.value = [droppedFiles[0]]
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

    if (response) {
      response.imageUrl = null;
    }
    recipe.value = response;
  } catch (error) {
    console.error("Error:", error);
  } finally {
    loading.value = false;
  }
}

const saveToCollection = async () => {
  if (!recipe.value || hasSaved.value) return;

  saving.value = true;

  const isAuth = await isAuthenticated();

  if (!isAuth) {
    saving.value = false;
    localStorage.setItem('pending_recipe', JSON.stringify(recipe.value))
    router.push('/login')
    return;
  }

  try {
    const response = await $fetch(`${config.public.apiBase}api/recipe/save`, {
      method: 'POST',
      credentials: 'include',
      body: recipe.value
    });

    if (response && response.id) {
      recipe.value.id = response.id
    }

    hasSaved.value = true;

  } catch (error) {
    console.error("Save failed:", error);
  } finally {
    saving.value = false;
  }
}

const clearAll = () => {
  files.value = null
  recipe.value = null
  hasSaved.value = false
  loading.value = false
  saving.value = false
  localStorage.removeItem('pending_recipe')
}
</script>