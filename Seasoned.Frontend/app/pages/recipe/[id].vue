<template>
  <div class="recipe-bg min-h-screen">
    <v-container>
       <RecipeDisplay v-if="recipe" :recipe="recipe" :is-public-view="true" />
       <v-progress-linear v-else indeterminate color="#8c4a32" />
    </v-container>
  </div>
</template>

<script setup>
const route = useRoute();
const config = useRuntimeConfig();

const { data: recipe, error } = await useAsyncData(`recipe-${route.params.id}`, () => 
  $fetch(`${config.public.apiBase}api/recipe/${route.params.id}`)
)

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