export default defineNuxtConfig({
  future: {
    compatibilityVersion: 4,
  },
  ssr: false,
  srcDir: 'app/',
  dir: {
    public: 'public/'
  },
  modules: [
    'vuetify-nuxt-module'
  ],
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  build: {
    transpile: ['vuetify'],
  },
  app: {
    baseURL: '/',
    buildAssetsDir: '_nuxt',
    head: {
      title: 'Seasoned',
      meta: [
        { property: 'og:site_name', content: 'Seasoned' },
        { property: 'og:title', content: 'AI powered recipe app' },
        { property: 'og:description', content: 'Discover and preserve tradtions in one place.' },
        { property: 'og:image', content: 'https://seasoned.ddns.net/images/image-preview.png' },
        { property: 'og:type', content: 'website' },
        { name: 'twitter:card', content: 'summary_large_image' }
      ],
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
    '@/assets/css/app-theme.css',
    '@/assets/css/gallery.css',
    '@/assets/css/login.css',
  ],
  runtimeConfig: {
    public: {
      apiBase: '' 
    }
  },
})
