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
  const waiting = ref(false)
  const errMessage = ref<string | null>(null)
  const inputErrors = ref<Record<string, string[]>>({})
  const isAuthenticated = computed(() => !!token.value)

  async function login(username: string, password: string) {
    waiting.value = true
    errMessage.value = null

    try {
      const response = await fetch(`${API_URL}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      })

      if (!response.ok) {
        const errData = await response.json()
        errMessage.value = errData.message || 'Login failed'
        if (errData.errors) {
          inputErrors.value = errData.errors
        }
        waiting.value = false
        return
      }

      const data: LoginResponse = await response.json()

      token.value = data.token
      localStorage.setItem('token', data.token)

      waiting.value = false
    } catch (err: unknown) {
      if (err instanceof Error) {
        errMessage.value = err.message || 'Unknown error occurred'
      }

      waiting.value = false
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
    waiting,
    errMessage,
    inputErrors,
    isAuthenticated,
    login,
    logout,
    initialize,
  }
})
