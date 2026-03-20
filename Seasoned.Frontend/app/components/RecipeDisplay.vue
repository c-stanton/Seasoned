<template>
  <transition name="fade">
    <div v-if="recipe" class="recipe-content">
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

      <v-expand-transition>
        <v-card 
          v-if="showSavePrompt" 
          class="mx-auto mb-8 save-prompt-paper"
          max-width="600"
          elevation="4"
        >
          <div class="d-flex align-center pa-5">
            <v-icon color="#8c4a32" class="mr-4" size="large">mdi-feather</v-icon>
            
            <div class="text-left">
              <p class="prompt-title mb-1">A Note from the Chef</p>
              <p class="prompt-text mb-0">
                To share this creation with others, please <strong>Save to Collection</strong> first.
              </p>
            </div>
            
            <v-spacer></v-spacer>
            
            <v-btn 
              icon="mdi-close" 
              variant="text" 
              size="small" 
              color="#5d4a36" 
              @click="showSavePrompt = false"
            ></v-btn>
          </div>
        </v-card>
      </v-expand-transition>

      <v-row justify="center" class="mt-12 pb-10">
        <v-btn
          class="px-8 print-btn"
          size="large"
          elevation="0"
          @click="printRecipe"
        >
          <v-icon icon="mdi-printer-outline" class="mr-2"></v-icon>
          Print Recipe
        </v-btn>

        <v-btn
          v-if="!isPublicView"
          class="px-8 transition-swing"
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

        <v-btn
          class="px-8 transition-swing"
          size="large"
          elevation="0"
          :color="hasShared ? '#556b2f' : '#5d4a36'"
          :class="hasShared ? 'share-success-btn' : 'share-btn'"
          @click="shareRecipe"
        >
          <template v-if="!hasShared">
            <v-icon icon="mdi-share-variant-outline" class="mr-2"></v-icon>
            Share Recipe
          </template>
          
          <template v-else>
            <v-icon icon="mdi-content-save-check-outline" class="mr-2"></v-icon>
            Link Copied!
          </template>
        </v-btn>
      </v-row>
      <v-fade-transition>
        <div v-if="isPublicView" class="public-cta-container mt-16 text-center pa-8">
          <v-divider class="mb-8 separator"></v-divider>
          <h3 class="cta-title mb-4">Enjoyed this recipe?</h3>
          <p class="cta-text mb-6">
            Join <strong>Seasoned</strong> to scan your own family recipes, 
            consult with the Chef, and build your digital archive.
          </p>
          <v-btn 
            to="/login" 
            class="auth-btn px-10" 
            size="large"
            elevation="4"
          >
            Start Your Collection
          </v-btn>
        </div>
      </v-fade-transition>
    </div>
  </transition>
</template>

<script setup>
import { ref } from 'vue'
import '@/assets/css/app-theme.css'
const hasShared =ref(false)
const showSavePrompt = ref(false)

const props = defineProps({
  recipe: { type: Object, default: null },
  isSaving: { type: Boolean, default: false },
  hasSaved: { type: Boolean, default: false },
  isPublicView: { type: Boolean, default: false }
})

defineEmits(['save'])

const printRecipe = () => {
  window.print()
}

const shareRecipe = async () => {
  if (!props.recipe?.id) {
    showSavePrompt.value = true;

    setTimeout(() => {
      showSavePrompt.value = false;
    }, 8000);
    return;
  }

  const shareUrl = `${window.location.origin}/recipe/${props.recipe.id}`;

  const shareData = {
    title: props.recipe.title,
    text: `Check out this delicious ${props.recipe.title} recipe on Seasoned!`,
    url: shareUrl
  }

  try {
    if (navigator.share) {
      await navigator.share(shareData);
    } else {
      await navigator.clipboard.writeText(shareUrl);
    }
    triggerShareSuccess();
  } catch (err) {
    if (err.name !== 'AbortError') console.error('Error sharing:', err);
  }
}

const triggerShareSuccess = () => {
  hasShared.value = true
  setTimeout(() => {
    hasShared.value = false
  }, 3000)
}
</script>