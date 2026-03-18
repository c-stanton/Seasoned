// vitest.config.ts
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'jsdom',
    css: false, 
    server: {
      deps: {
        inline: [
          /@exodus\/bytes/, 
          /html-encoding-sniffer/,
          /vuetify/
        ],
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './app'),
      '~': path.resolve(__dirname, './app')
    },
  },
})