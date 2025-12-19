<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const API_URL = import.meta.env.VITE_API_BASE_URL

const router = useRouter()
const username = ref('')
const password = ref('')
const confirmPassword = ref('')
const waiting = ref(false)
const errMessage = ref<string | null>(null)
const inputErrors = ref<Record<string, string[]>>({})

async function handleRegister() {
  waiting.value = true
  errMessage.value = null

  try {
    const response = await fetch(`${API_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
        confirmPassword: confirmPassword.value,
      }),
    })

    if (!response.ok) {
      const errData = await response.json()
      errMessage.value = errData.message || 'Registration failed'
      if (errData.errors) {
        inputErrors.value = errData.errors
      }
      waiting.value = false
      return
    }

    router.push('/login')
  } catch (e: unknown) {
    if (e instanceof Error) {
      errMessage.value = e.message || 'Unknown error occurred'
    }

    waiting.value = false
  }
}
</script>

<template>
  <div class="flex min-h-full flex-col py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <img
        class="mx-auto h-10 w-auto"
        src="https://tailwindcss.com/plus-assets/img/logos/mark.svg?color=teal&shade=600"
        alt="Your Company"
      />
      <h2 class="mt-6 text-center text-2xl/9 font-bold tracking-tight text-gray-900">Note App</h2>
    </div>

    <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-[480px]">
      <div class="bg-white px-4 py-8 shadow-sm sm:rounded-lg sm:px-12">
        <p v-if="errMessage" class="text-red-500 text-sm">{{ errMessage }}</p>

        <form @submit.prevent="handleRegister" class="space-y-6">
          <div>
            <label for="username" class="block text-sm/6 font-medium text-gray-900">Username</label>
            <div class="mt-2">
              <input
                type="text"
                v-model="username"
                id="username"
                autocomplete="username"
                class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:text-sm/6"
              />
            </div>
            <div v-if="inputErrors.username" class="mt-1 text-red-500 text-sm">
              <span v-for="(error, index) in inputErrors.username" :key="index">{{ error }}</span>
            </div>
          </div>

          <div>
            <label for="password" class="block text-sm/6 font-medium text-gray-900">Password</label>
            <div class="mt-2">
              <input
                type="password"
                v-model="password"
                id="password"
                class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:text-sm/6"
              />
            </div>
            <div v-if="inputErrors.password" class="mt-1 text-red-500 text-sm">
              <span v-for="(error, index) in inputErrors.password" :key="index">{{ error }}</span>
            </div>
          </div>

          <div>
            <label for="confirm_password" class="block text-sm/6 font-medium text-gray-900"
              >Confirm Password</label
            >
            <div class="mt-2">
              <input
                type="password"
                v-model="confirmPassword"
                id="confirm_password"
                class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:text-sm/6"
              />
            </div>
            <div v-if="inputErrors.confirmPassword" class="mt-1 text-red-500 text-sm">
              <span v-for="(error, index) in inputErrors.confirmPassword" :key="index">{{
                error
              }}</span>
            </div>
          </div>

          <div>
            <button
              type="submit"
              :disabled="waiting"
              class="cursor-pointer flex w-full justify-center rounded-md bg-teal-600 px-3 py-2 text-sm/6 font-semibold text-white shadow-xs hover:bg-teal-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-teal-600"
            >
              {{ waiting ? 'Registering...' : 'Register' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
