<template>
  <div class="recipe-bg min-h-screen">
    <v-container>
       <RecipeDisplay v-if="normalizedRecipe" :recipe="normalizedRecipe" :is-public-view="true" />
       <v-progress-linear v-else indeterminate color="#8c4a32" />
    </v-container>
  </div>
</template>

<script setup>
const route = useRoute();
const config = useRuntimeConfig();

const { data: recipe, error } = await useAsyncData(`recipe-${route.params.id}`, () => {
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

if (error.value || !recipe.value) {
  throw createError({ statusCode: 404, statusMessage: 'Recipe not found' })
}

useSeoMeta({
  title: `${recipe.value.title} | Seasoned`,
  ogTitle: `Chef's Choice: ${recipe.value.title}`,
  description: `Check out this delicious recipe for ${recipe.value.title} on Seasoned.`,
  ogDescription: `A hand-crafted parchment recipe for ${recipe.value.title}.`,
  ogImage: recipe.value.imageUrl || '/images/seasoned-logo.png',
  twitterCard: 'summary_large_image',
  ogType: 'article',
})
</script>