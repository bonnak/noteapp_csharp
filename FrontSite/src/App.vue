<script setup lang="ts">
import HeaderNav from './components/HeaderNav.vue'
</script>

<template>
  <div class="h-full flex flex-col flex-grow lg:pl-72">
    <HeaderNav />

    <RouterView />
  </div>
</template>
