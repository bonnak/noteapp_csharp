<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

function logout() {
  auth.logout()

  if (!auth.isAuthenticated) {
    router.push('/login')
  }
}
</script>

<template>
  <div
    class="sticky top-0 z-40 flex h-16 shrink-0 items-center gap-x-4 border-b border-gray-200 bg-gray-100 px-4 sm:gap-x-6 sm:px-6 lg:px-8"
  >
    <!-- Separator -->
    <div class="h-6 w-px bg-gray-200 lg:hidden" aria-hidden="true"></div>

    <div class="flex flex-1 gap-x-4 self-stretch lg:gap-x-6">
      <div class="flex-1"></div>
      <div v-if="!useAuthStore().isAuthenticated" class="flex items-center gap-x-4 lg:gap-x-6">
        <RouterLink
          to="/login"
          class="text-sm font-semibold leading-6 text-gray-900 hover:text-gray-700"
        >
          Log in
        </RouterLink>
        <RouterLink
          to="/register"
          class="text-sm font-semibold leading-6 text-gray-900 hover:text-gray-700"
        >
          Register
        </RouterLink>
      </div>
      <div v-else class="flex items-center gap-x-4 lg:gap-x-6">
        <!-- Separator -->
        <div class="hidden lg:block lg:h-6 lg:w-px lg:bg-gray-200" aria-hidden="true"></div>

        <!-- Logout -->
        <button
          @click="logout"
          type="button"
          class="cursor-pointer text-sm font-semibold leading-6 text-gray-900 hover:text-gray-700"
        >
          Log out
        </button>
      </div>
    </div>
  </div>
</template>
