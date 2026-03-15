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
        <p class="brand-subtitle">Kitchen Consultation</p>
      </header>
      
      <v-divider class="mb-10 separator"></v-divider>

      <v-row justify="center" class="mb-6">
        <v-col cols="12" md="10">
          <div class="chat-container">
            <div class="section-header mb-4 d-flex align-center">
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
            </div>

            <v-textarea
                v-model="userQuery"
                variant="outlined"
                auto-grow
                rows="1"
                max-rows="6"
                hide-details
                class="chat-input"
                @keyup.enter.exact.prevent="askChef"
                :loading="chatLoading"
                >
                <template v-slot:append-inner>
                    <v-btn 
                    icon="mdi-send-variant" 
                    variant="text" 
                    size="small" 
                    color="#8c4a32" 
                    class="mt-1"
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
const saving = ref(false)
const hasSaved = ref(false)

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