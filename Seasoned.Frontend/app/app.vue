<template>
  <v-app>
    <v-main>
      <v-container>
        <v-card class="pa-5 mx-auto mt-10" max-width="500" elevation="10">
          <v-card-title class="text-center">Seasoned AI</v-card-title>
          
          <v-divider class="my-3"></v-divider>

          <v-file-input
            v-model="files"
            label="Pick a recipe photo"
            prepend-icon="mdi-camera"
            variant="outlined"
            accept="image/*"
          ></v-file-input>

          <v-btn 
            color="primary" 
            block 
            size="x-large"
            :loading="loading"
            @click="uploadImage"
          >
            Analyze Recipe
          </v-btn>

          <div v-if="recipe" class="mt-5">
            <h2 class="text-h4 mb-4">{{ recipe.title }}</h2>
            <p class="text-subtitle-1 mb-6 text-grey-darken-1">{{ recipe.description }}</p>

            <v-row>
              <v-col cols="12" md="5">
                <h3 class="text-h6 mb-2">Ingredients</h3>
                <v-list lines="one" variant="flat" class="bg-grey-lighten-4 rounded-lg">
                  <v-list-item v-for="(item, i) in recipe.ingredients" :key="i">
                    <template v-slot:prepend>
                      <v-icon icon="mdi-circle-small"></v-icon>
                    </template>
                    {{ item }}
                  </v-list-item>
                </v-list>
              </v-col>

              <v-col cols="12" md="7">
                <h3 class="text-h6 mb-2">Instructions</h3>
                <v-timeline side="end" align="start" density="compact">
                  <v-timeline-item
                    v-for="(step, i) in recipe.instructions"
                    :key="i"
                    dot-color="primary"
                    size="x-small"
                  >
                    <div class="text-body-1">{{ step }}</div>
                  </v-timeline-item>
                </v-timeline>
              </v-col>
            </v-row>
          </div>
        </v-card>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import axios from 'axios'
import { ref } from 'vue'

const files = ref([])
const loading = ref(false)
const recipe = ref(null)

const uploadImage = async () => {
  // 1. Debug: Check what Vuetify is actually giving us
  console.log("Files variable:", files.value);

  // Vuetify 3 v-file-input can return a single File or an Array of Files
  // We need to ensure we have the actual File object
  const fileToUpload = Array.isArray(files.value) ? files.value[0] : files.value;

  if (!fileToUpload) {
    alert("Please select a file first!");
    return;
  }

  loading.value = true;
  const formData = new FormData();
  
  // 2. Append the file. The string 'image' MUST match your C# parameter name
  formData.append('image', fileToUpload);

  try {
    // 3. Post with explicit multipart/form-data header (Axios usually does this, but let's be sure)
    const response = await axios.post('http://localhost:5000/api/recipe/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    
    recipe.value = response.data;
    console.log("Success:", response.data);
  } catch (error) {
    console.error("Detailed Error:", error.response?.data || error.message);
    alert("Backend error: Check the browser console for details.");
  } finally {
    loading.value = false;
  }
}
</script>