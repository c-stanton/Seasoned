<template>
  <v-container>
    <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="950" elevation="1">
      
      <header class="text-center mb-10">
        <div class="brand-icon-container mb-4">
          <v-img 
            :src="'/images/seasoned-logo.png'"
            alt="Seasoned Logo"
            width="120"
            class="auth-logo mx-auto"
            cover
          ></v-img>
        </div>
        <p class="brand-subtitle">Recipe Creator and Recipe Uploader</p>
      </header>
      
      <v-divider class="mb-10 separator"></v-divider>

      <v-row justify="center" class="mb-6">
        <v-col cols="12" md="8">
          <div class="chat-container">
            <div class="section-header mb-4 d-flex align-center">
              <v-icon icon="mdi-chef-hat" class="mr-2" size="small"></v-icon>
              <span>Kitchen Consultation</span>
              <v-spacer></v-spacer>
              <v-btn 
                v-if="chatMessages.length > 0"
                icon="mdi-delete-sweep-outline" 
                variant="text" 
                size="x-small" 
                color="#8c7e6a"
                title="Clear Conversation"
                @click="chatMessages = []"
              ></v-btn>
            </div>
            
            <div class="chat-display mb-4" ref="chatDisplay">
              <div v-if="chatMessages.length === 0" class="chat-placeholder">
                "What shall we create today?"
              </div>
              <div v-for="(msg, i) in chatMessages" :key="i" :class="['message', msg.role]">
                <span class="message-text">{{ msg.text }}</span>
              </div>
            </div>

            <v-text-field
              v-model="userQuery"
              variant="outlined"
              hide-details
              class="chat-input"
              @keyup.enter="askChef"
              :loading="chatLoading"
            >
              <template v-slot:append-inner>
                <v-btn icon="mdi-send-variant" variant="text" size="small" color="#5d4037" @click="askChef"></v-btn>
              </template>
            </v-text-field>
          </div>
        </v-col>
      </v-row>

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

    <v-snackbar
      v-model="snackbar.show"
      :timeout="4000"
      :color="snackbar.color"
      class="thematic-snackbar"
      location="bottom"
    >
      <div class="d-flex align-center">
        <v-icon :icon="snackbar.icon" :color="snackbar.iconColor" class="mr-3"></v-icon>
        <span class="snackbar-text" :style="{ color: snackbar.textColor }">
          {{ snackbar.message }}
        </span>
      </div>
    </v-snackbar>

  </v-container>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
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
const userQuery = ref('')
const chatLoading = ref(false)
const chatMessages = ref([])
const chatDisplay = ref(null)

onMounted(() => {
  const savedRecipe = localStorage.getItem('pending_recipe')
  if (savedRecipe) {
    recipe.value = JSON.parse(savedRecipe)
    localStorage.removeItem('pending_recipe')
    
    snackbar.value = {
      show: true,
      message: 'Restored your analyzed recipe.',
      color: '#f4ede1',
      icon: 'mdi-history',
      iconColor: '#556b2f',
      textColor: '#5d4037'
    }
  }
})

const isAuthenticated = async () => {
 try {
    await $fetch('/api/auth/manage/info', {
      credentials: 'include'
    })
    return true
  } catch {
    return false
  }
}

const handleViewCollection = () => {
  const token = useCookie('seasoned_token').value 
                || (import.meta.client ? localStorage.getItem('token') : null)
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

  saving.value = true;

  const isAuth = await isAuthenticated();

  if (!isAuth) {
    saving.value = false;
    localStorage.setItem('pending_recipe', JSON.stringify(recipe.value))
    
    snackbar.value = {
      show: true,
      message: 'Please sign in to preserve this recipe in your archives.',
      color: '#efe5e3',
      icon: 'mdi-account-key',
      iconColor: '#8c4a32',
      textColor: '#5d4037'
    };

    setTimeout(() => {
      router.push('/login')
    }, 2000)
    return;
  }

  try {
    await $fetch(`${config.public.apiBase}api/recipe/save`, {
      method: 'POST',
      credentials: 'include',
      body: recipe.value
    });

    hasSaved.value = true;
    snackbar.value = {
      show: true,
      message: 'Recipe added to your collection.',
      color: '#f4ede1',
      icon: 'mdi-check-decagram',
      iconColor: '#556b2f',
      textColor: '#5d4037'
    };
  } catch (error) {
    console.error("Save failed:", error);
    snackbar.value = {
      show: true,
      message: 'Failure to save recipe.',
      color: '#f8d7da',
      icon: 'mdi-alert-rhombus',
      iconColor: '#8c4a32',
      textColor: '#5d4037'
    };
  } finally {
    saving.value = false;
  }
}

const snackbar = ref({
  show: false,
  message: '',
  color: '#f4ede1',
  icon: 'mdi-check-decagram',
  iconColor: '#556b2f',
  textColor: '#5d4037'
})

const clearAll = () => {
  files.value = []
  recipe.value = null
  hasSaved.value = false
  loading.value = false
  saving.value = false
  localStorage.removeItem('pending_recipe')
}

const askChef = async () => {
  if (!userQuery.value.trim()) return

  const query = userQuery.value
  chatMessages.value.push({ role: 'user', text: userQuery.value })
  userQuery.value = ''
  chatLoading.value = true

  await nextTick()
  scrollToBottom()

  try {
    const data = await $fetch(`${config.public.apiBase}api/recipe/consult`, {
      method: 'POST',
      body: { prompt: query }
    })

    chatMessages.value.push({ role: 'assistant', text: data.reply })

    if (data.recipe && data.recipe.title) {
      recipe.value = data.recipe
      hasSaved.value = false
      files.value = []
      localStorage.removeItem('pending_recipe')
    }

    await nextTick()
    scrollToBottom()

  } catch (err) {
    chatMessages.value.push({ 
      role: 'assistant', 
      text: "The kitchen is currently closed for repairs. Try again in a moment?" 
    })
  } finally {
    chatLoading.value = false
  }
}

const scrollToBottom = () => {
  if (chatDisplay.value) {
    chatDisplay.value.scrollTop = chatDisplay.value.scrollHeight
  }
}
</script>