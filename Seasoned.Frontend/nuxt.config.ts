export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',

  devtools: { enabled: true },

  future: {
    compatibilityVersion: 4,
  },

  srcDir: 'app/',

  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css',
    '~/assets/css/app-theme.css',
    '~/assets/css/gallery.css',
    '~/assets/css/auth.css'
  ],

  build: {
    transpile: ['vuetify'],
  },

  modules: [
    'vuetify-nuxt-module'
  ],

  runtimeConfig: {
    public: {
      apiBase: '' 
    }
  },

  vite: {
    server: {
      hmr: process.env.NODE_ENV !== 'production' ? {
        protocol: 'ws',
        host: 'localhost',
        port: 3000
      } : undefined
    }
  }
})
