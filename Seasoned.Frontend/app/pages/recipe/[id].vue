<template>
  <div class="recipe-bg">
    <v-container class="py-10 d-flex justify-center"> <RecipeDisplay 
        v-if="normalizedRecipe" 
        :recipe="normalizedRecipe" 
        :is-public-view="true" 
        class="recipe-card"
        style="width: 100%; max-width: 1000px;" 
      />
      <v-progress-linear v-else indeterminate color="#8c4a32" />
    </v-container>
  </div>
</template>

<script setup>
import { computed } from 'vue'
definePageMeta({
  auth: false
})
const route = useRoute();
const config = useRuntimeConfig();

const { data: rawRecipe, error } = await useAsyncData(`recipe-${route.params.id}`, () => {
  const baseUrl = config.public.apiBase.endsWith('/') 
    ? config.public.apiBase 
    : `${config.public.apiBase}/`;
    
  return $fetch(`${baseUrl}api/recipe/${route.params.id}`);
});

const normalizedRecipe = computed(() => {
  if (!rawRecipe.value) return null;
  
  const r = rawRecipe.value;
  return {
    id: r.id || r.Id,
    title: r.title || r.Title || 'Untitled Recipe',
    ingredients: r.ingredients || r.Ingredients || [],
    instructions: r.instructions || r.Instructions || [],
    imageUrl: r.imageUrl || r.ImageUrl || null
  };
});

if (error.value || !normalizedRecipe.value) {
  throw createError({ statusCode: 404, statusMessage: 'Recipe not found' })
}

useSeoMeta({
  title: `${normalizedRecipe.value.title} | Seasoned`,
  ogTitle: `Chef's Choice: ${normalizedRecipe.value.title}`,
  description: `Check out this delicious recipe for ${normalizedRecipe.value.title} on Seasoned.`,
  ogImage: normalizedRecipe.value.imageUrl || '/images/seasoned-logo.png',
  twitterCard: 'summary_large_image',
  ogType: 'article',
})
</script>