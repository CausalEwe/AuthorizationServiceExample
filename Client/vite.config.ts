import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    }
  },
  server: {
    watch: {
      usePolling: true,
    },
    headers: {
    },
    host: true,
    port: 3000,
    proxy: {
      '/api/auth': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/auth/,  '/auth')
      },
      '/api/users': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/users/, '/users'),
      },
    },
  }
})
