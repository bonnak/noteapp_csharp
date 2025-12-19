import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/Register.vue'),
      meta: { redirectIfAuthenticated: true },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/Login.vue'),
      meta: { redirectIfAuthenticated: true },
    },
    {
      path: '/',
      name: 'home',
      component: Home,
      meta: {
        requireAuth: true,
      },
    },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  if (to.meta.requireAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.redirectIfAuthenticated && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
