import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss()
  ],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5266',
        changeOrigin: true,
        secure: false
      },
      '/images': {
        target: 'http://localhost:5266',
        changeOrigin: true,
        secure: false
      },
      '/hubs': {
        target: 'http://localhost:5266',
        changeOrigin: true,
        ws: true //WebSocket za SignalR??
      }
    }
  }
})
