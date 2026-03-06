export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',

  devtools: { enabled: true },

  future: {
    compatibilityVersion: 4,
  },

  srcDir: 'app/',

  app: {
    baseURL: '/',
    buildAssetsDir: '_nuxt',
    head: {
      title: 'Seasoned',
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/apple-touch-icon.png' },
        { rel: 'icon', type: 'image/png', sizes: '96x96', href: '/favicon-96x96.png' },
        { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/web-app-manifest-192x192.png' },
        { rel : 'icon', type: 'image/png', sizes: '512x512', href: '/web-app-manifest-512x512.png' },   
        { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' },    
        { rel: 'manifest', href: '/site.webmanifest' } 
      ]
    }
  },

  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css',
    '~/assets/css/app-theme.css',
    '~/assets/css/gallery.css',
    '~/assets/css/login.css'
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
