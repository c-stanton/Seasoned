<template>
  <v-container>
    <v-card class="recipe-card pa-6 mx-auto mt-4" max-width="950" elevation="1">
      <header class="text-center mb-4">
        <v-img 
            src="/images/seasoned-logo.png" 
            width="180" 
            class="mx-auto"
            contain
          >
        </v-img>
        <p class="chat-title">Kitchen Consultation</p>
      </header>
      
      <v-divider class="mb-6 separator"></v-divider>

      <v-row justify="center" class="mb-6">
        <v-col cols="12" md="11">
          <div class="chat-container">
            <div class="section-header mb-4 d-flex align-center">
              <v-spacer></v-spacer>
              <v-icon icon="mdi-chef-hat" class="mr-2" size="small"></v-icon>
              <span>Ask the Chef</span>
              <v-spacer></v-spacer>
              <v-btn v-if="chatMessages.length > 0" icon="mdi-delete-sweep-outline" variant="text" color="#8c7e6a" @click="chatMessages = []"></v-btn>
            </div>
            
            <div class="chat-display" ref="chatDisplay">
              <div v-if="chatMessages.length === 0" class="chat-placeholder">"What shall we create today?"</div>
              <div v-for="(msg, i) in chatMessages" :key="i" :class="['message', msg.role]">
                <span class="message-text">{{ msg.text }}</span>
              </div>
              <div v-if="chatLoading" class="message assistant thinking-bubble">
                <div class="typing">
                  <span class="dot"></span>
                  <span class="dot"></span>
                  <span class="dot"></span>
                </div>
              </div>
            </div>

            <v-textarea
                v-model="userQuery"
                variant="outlined"
                auto-grow
                rows="1"
                max-rows="6"
                hide-details
                class="chat-input"
                @keydown.enter.exact.prevent="askChef"
                :loading="chatLoading"
                >
                <template v-slot:append-inner>
                    <v-btn 
                    icon="mdi-send-variant" 
                    variant="text" 
                    class="mt-1 send-btn"
                    @click="askChef"
                    ></v-btn>
                </template>
            </v-textarea>
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
import { ref, nextTick } from 'vue'
import '@/assets/css/app-theme.css'

const config = useRuntimeConfig()
const recipe = ref(null)
const userQuery = ref('')
const chatLoading = ref(false)
const chatMessages = ref([])
const chatDisplay = ref(null)
const router = ref(false)
const saving = ref(false)
const hasSaved = ref(false)

const isAuthenticated = async () => {
  try {
    await $fetch('/api/auth/manage/info', { credentials: 'include' })
    return true
  } catch {
    return false
  }
}

const saveToCollection = async () => {
  if (!recipe.value || hasSaved.value) return

  saving.value = true

  try {
    const response = await $fetch(`${config.public.apiBase}api/recipe/save`, {
      method: 'POST',
      credentials: 'include',
      body: recipe.value
    })

    if (response && response.id) {
      recipe.value.id = response.id
    }

    hasSaved.value = true
  } catch (error) {
    console.error("Save failed:", error)
  } finally {
    saving.value = false
  }
}

const askChef = async () => {
  if (!userQuery.value.trim()) return

  const query = userQuery.value
  chatMessages.value.push({ role: 'user', text: userQuery.value })
  userQuery.value = ''
  chatLoading.value = true

  await scrollToBottom()

  try {
    const data = await $fetch(`${config.public.apiBase}api/recipe/consult`, {
      method: 'POST',
      body: { prompt: query }
    })

    chatMessages.value.push({ role: 'assistant', text: data.reply })

    if (data.recipe && data.recipe.title) {
      recipe.value = data.recipe
      hasSaved.value = false
      localStorage.removeItem('pending_recipe')
    }

  } catch (err) {
    chatMessages.value.push({ 
      role: 'assistant', 
      text: "The kitchen is currently closed for repairs. Try again in a moment?" 
    })
  } finally {
    chatLoading.value = false
    await scrollToBottom()
  }
}

const scrollToBottom = async () => {
  await nextTick()
  if (chatDisplay.value) {
    const { scrollTop, scrollHeight, clientHeight } = chatDisplay.value
    const isAtBottom = scrollHeight - scrollTop <= clientHeight + 100

    if (isAtBottom) {
      chatDisplay.value.scrollTo({
        top: scrollHeight,
        behavior: 'smooth'
      })
    }
  }
}
</script>