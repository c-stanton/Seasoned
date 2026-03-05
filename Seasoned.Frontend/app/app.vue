<template>
  <v-app class="recipe-bg">
    <v-main>
      <v-container>
        <v-card class="recipe-card pa-10 mx-auto mt-10" max-width="950" elevation="1">
          
          <header class="text-center mb-10">
            <h1 class="brand-title">Seasoned</h1>
            <p class="brand-subtitle">A Recipe Collection</p>
          </header>
          
          <v-divider class="mb-10 separator"></v-divider>

          <v-row justify="center" class="mb-12">
            <v-col cols="12" md="8">
              <v-file-input
                v-model="files"
                label="Upload Image"
                variant="solo-filled"
                flat
                accept="image/*"
                class="custom-input mb-4"
              ></v-file-input>

              <v-btn 
                class="analyze-btn"
                block 
                size="x-large"
                elevation="0"
                :loading="loading"
                @click="uploadImage"
              >
                Analyze Recipe
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
            </div>
          </transition>
        </v-card>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import axios from 'axios'
import { ref } from 'vue'
import '@/assets/css/app-theme.css'
const config = useRuntimeConfig()
const files = ref([])
const loading = ref(false)
const recipe = ref(null)

const uploadImage = async () => {
  const fileToUpload = Array.isArray(files.value) ? files.value[0] : files.value;
  if (!fileToUpload) return;

  loading.value = true;
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
</script>