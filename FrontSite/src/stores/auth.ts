import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

const API_URL = import.meta.env.VITE_API_BASE_URL

interface User {
  id: number
  username: string
}

interface LoginResponse {
  token: string
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const isAuthenticated = computed(() => !!token.value)

  async function login(username: string, password: string) {
    loading.value = true
    error.value = null

    try {
      const response = await fetch(`${API_URL}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      })

      if (!response.ok) {
        const errData = await response.json()
        throw new Error(errData.message || 'Login failed')
      }

      const data: LoginResponse = await response.json()

      token.value = data.token

      localStorage.setItem('token', data.token)
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
  }

  function initialize() {
    const storedToken = localStorage.getItem('token')
    if (storedToken) {
      token.value = storedToken
    }
  }

  return {
    token,
    user,
    loading,
    error,
    isAuthenticated,
    login,
    logout,
    initialize,
  }
})
