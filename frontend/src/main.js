import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth'//new

/*createApp(App)
  .use(createPinia())
  .use(router)
  .mount('#app')*/

const app = createApp(App)
const pinia = createPinia()
app.use(pinia)
app.use(router)

const auth = useAuthStore(pinia)
if (auth.isAuthenticated) {
    auth.verifyToken().finally(() => app.mount('#app'))
} else {
    app.mount('#app')
}