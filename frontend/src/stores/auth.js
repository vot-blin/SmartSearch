import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import http from '@/api/http' //new

export const useAuthStore = defineStore('auth', () => {
    const user = ref(JSON.parse(localStorage.getItem('user') || null))
    const token = ref(localStorage.getItem('token') || '')
    const checked = ref(false) //new

    const isAuthenticated = computed(() => !!token.value)

    function login(payload) {
        user.value = payload.user
        token.value = payload.token
        localStorage.setItem('user', JSON.stringify(payload.user))
        localStorage.setItem('token', payload.token)
    }

    function logout() {
        user.value = null
        token.value = ''
        localStorage.removeItem('user')
        localStorage.removeItem('token')
    }

    //new block
    async function verifyToken() {
        if (!token.value) {
            checked.value = true
            return false
        }
        try {
            const res = await http.get('/auth/me')
            if (res.data) {
                user.value = res.data
                localStorage.setItem('user', JSON.stringify(res.data))
                checked.value = true
                return true
            }
        } catch (e) {
            const status = e.response?.status
            if (status === 401 || status === 403 || !status) {
                logout()
            }
        }
        checked.value = true
        return false
    }

    return { user, token, checked, isAuthenticated, login, logout, verifyToken }
})