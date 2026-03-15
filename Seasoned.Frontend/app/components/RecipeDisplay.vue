<template>
  <transition name="fade">
    <div v-if="recipe" class="recipe-content">
      <v-divider class="mb-10 separator"></v-divider>
      
      <h2 class="recipe-title text-center mb-4">{{ recipe.title }}</h2>
      <p v-if="recipe.description" class="recipe-description text-center mb-12 text-italic">
        {{ recipe.description }}
      </p>

      <v-row>
        <v-col cols="12" md="5">
          <div class="section-header mb-6 px-2">
            <v-icon icon="mdi-spoon-sugar" class="mr-2" size="small"></v-icon>
            <span>Ingredients</span>
          </div>
          <div class="ingredients-container">
            <div v-for="(item, i) in recipe.ingredients" :key="i" class="ingredient-item d-flex align-center">
                {{ item }}
            </div>
          </div>
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
          :loading="isSaving"
          @click="$emit('save')"
        >
          <v-icon icon="mdi-content-save-check-outline" class="mr-2"></v-icon>
          Save to Collection
        </v-btn>
        
        <v-chip
          v-else
          color="#556b2f"
          variant="outlined"
          prepend-icon="mdi-check-decagram"
          class="pa-6"
        >
          Saved to Archives
        </v-chip>
      </v-row>
    </div>
  </transition>
</template>

<script setup>
defineProps({
  recipe: { type: Object, default: null },
  isSaving: { type: Boolean, default: false },
  hasSaved: { type: Boolean, default: false }
})

defineEmits(['save'])
</script>