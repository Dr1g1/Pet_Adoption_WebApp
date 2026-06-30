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
      // svaki pozizv na /api ide na nas c# backend
      // tako izbegavamo CORS problem tokom razvoja
      '/api': {
        target: 'http://localhost:5266',
        changeOrigin: true
      },
      '/images': {
        target: 'http://localhost:5266',
        changeOrigin: true
      },
      '/hubs': {
        target: 'http://localhost:5266',
        changeOrigin: true,
        ws: true //WebSocket za SignalR??
      }
    }
  }
})
