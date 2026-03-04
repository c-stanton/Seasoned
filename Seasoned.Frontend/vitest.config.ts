// vitest.config.ts
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'jsdom',
    server: {
      deps: {
        inline: [/@exodus\/bytes/, /html-encoding-sniffer/],
      },
    },
  },
})