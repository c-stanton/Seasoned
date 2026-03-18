<template>
  <transition name="fade">
    <div v-if="recipe" class="recipe-content mx-auto" style="max-width: 900px;">
      <v-divider class="mb-10 separator"></v-divider>
      
      <h2 class="recipe-title text-center mb-4">{{ recipe.title }}</h2>
      <p v-if="recipe.description" class="recipe-description text-center mb-16 text-italic">
        {{ recipe.description }}
      </p>

      <v-row class="mt-10" no-gutters>
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
import { ref } from 'vue'
import '@/assets/css/app-theme.css'

defineProps({
  //recipe: { type: Object, default: null },
  isSaving: { type: Boolean, default: false },
  hasSaved: { type: Boolean, default: false }
})

defineEmits(['save'])

const printRecipe = () => {
  window.print()
}

// mock output
const recipe = ref({
  title: "Bakery-Style Lemon Blueberry Muffins",
  ingredients: [
    "2 cups all-purpose flour",
    "1/2 cup granulated sugar",
    "2 tsp baking powder",
    "1/2 tsp salt",
    "1/2 cup unsalted butter, melted",
    "2 large eggs",
    "1/2 cup whole milk",
    "1 tbsp fresh lemon zest",
    "2 tbsp fresh lemon juice",
    "1 1/2 cups fresh blueberries"
  ],
  instructions: [
    "Preheat your oven to 375°F (190°C) and line a muffin tin with paper liners.",
    "In a large bowl, whisk together the flour, sugar, baking powder, and salt.",
    "In a separate bowl, whisk the melted butter, eggs, milk, lemon zest, and lemon juice.",
    "Pour the wet ingredients into the dry and stir gently until just combined; do not overmix or the muffins will be tough.",
    "Toss the blueberries in a teaspoon of flour, then gently fold them into the batter.",
    "Divide the batter evenly into the muffin cups and bake for 18-20 minutes until golden."
  ]
})
</script>