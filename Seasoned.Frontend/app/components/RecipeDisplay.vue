<template>
  <transition name="fade">
    <div v-if="recipe" class="recipe-content mx-auto" style="max-width: 900px;">
      <v-divider class="mb-10 separator"></v-divider>
      
      <h2 class="recipe-title text-center mb-4">{{ recipe.title }}</h2>
      
      <v-img
        v-if="recipe.imageUrl"
        :src="recipe.imageUrl"
        class="recipe-image rounded-lg mb-8 mx-auto"
        elevation="2"
        max-height="400"
        cover
      ></v-img>

      <v-row class="mt-10" density="compact">
        <v-col cols="12" md="5" class="pe-md-10">
          <div class="section-header justify-center mb-6">
            <v-icon icon="mdi-spoon-sugar" class="mr-2" size="small"></v-icon>
            <span>Ingredients</span>
          </div>
          <div class="ingredients-container">
            <div v-for="(item, i) in recipe.ingredients" :key="i" class="ingredient-item">
                {{ item }}
            </div>
          </div>
        </v-col>

        <v-col cols="12" md="7" class="ps-md-10">
          <div class="section-header justify-center mb-6">
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
          class="print-btn px-12"
          size="large"
          elevation="0"
          @click="printRecipe"
        >
          <v-icon icon="mdi-printer-outline" class="mr-2"></v-icon>
          Print Recipe
        </v-btn>

        <v-btn
          class="px-12 transition-swing"
          size="large"
          elevation="0"
          :loading="isSaving"
          :disabled="hasSaved"
          :color="hasSaved ? '#556b2f' : '#5d4a36'"
          :class="hasSaved ? 'save-success-btn' : 'save-recipe-btn'"
          @click="$emit('save')"
        >
          <template v-if="!hasSaved">
            <v-icon icon="mdi-content-save-check-outline" class="mr-2"></v-icon>
            Save to Collection
          </template>
          
          <template v-else>
            <v-icon icon="mdi-check-all" class="mr-2"></v-icon>
            Saved in Archives
          </template>
        </v-btn>
      </v-row>
    </div>
  </transition>
</template>

<script setup>
import { ref } from 'vue'
import '@/assets/css/app-theme.css'

defineProps({
  recipe: { type: Object, default: null },
  isSaving: { type: Boolean, default: false },
  hasSaved: { type: Boolean, default: false }
})

defineEmits(['save'])

const printRecipe = () => {
  window.print()
}


</script>