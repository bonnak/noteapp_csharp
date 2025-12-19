<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
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
        <button type="button" class="-m-2.5 p-2.5 text-gray-500 hover:text-gray-700 cursor-pointer">
          <span class="sr-only">View notifications</span>
          <svg
            class="size-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke-width="1.5"
            stroke="currentColor"
            aria-hidden="true"
            data-slot="icon"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M14.857 17.082a23.848 23.848 0 0 0 5.454-1.31A8.967 8.967 0 0 1 18 9.75V9A6 6 0 0 0 6 9v.75a8.967 8.967 0 0 1-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 0 1-5.714 0m5.714 0a3 3 0 1 1-5.714 0"
            ></path>
          </svg>
        </button>

        <!-- Separator -->
        <div class="hidden lg:block lg:h-6 lg:w-px lg:bg-gray-200" aria-hidden="true"></div>
      </div>
    </div>
  </div>
</template>
