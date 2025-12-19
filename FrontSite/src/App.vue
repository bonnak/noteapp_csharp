<script setup lang="ts">
import HeaderNav from './components/HeaderNav.vue'
</script>

<template>
  <div class="h-full flex flex-col flex-grow">
    <HeaderNav />

    <RouterView />
  </div>
</template>
