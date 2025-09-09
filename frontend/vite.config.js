
import { defineConfig } from "vite";
import { createVuePlugin } from "vite-plugin-vue2";

export default defineConfig({
  plugins: [createVuePlugin()],
  server: {
    port: 5173,
    open: true
  },
  resolve: {
    alias: {
      'vue': 'vue/dist/vue.esm.js'
    }
  }
});
