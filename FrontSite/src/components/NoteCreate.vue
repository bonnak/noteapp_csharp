<script setup lang="ts">
import { ref } from 'vue'
import { useNoteStore, type Note } from '@/stores/note'
import { useAuthStore } from '@/stores/auth'

const API_URL = import.meta.env.VITE_API_BASE_URL

interface NoteResponse {
  note: Note
}

const noteStore = useNoteStore()
const auth = useAuthStore()

const waiting = ref(false)
const errMessage = ref<string | null>(null)
const inputErrors = ref<Record<string, string[]>>({})
const title = ref('')
const content = ref('')
const emit = defineEmits<{
  (e: 'note-created'): void
}>()

async function handleCreateNote() {
  waiting.value = true
  errMessage.value = null
  inputErrors.value = {}

  try {
    const response = await fetch(`${API_URL}/notes`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${auth.token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ title: title.value, content: content.value }),
    })

    if (!response.ok) throw new Error('Failed to create a new note')

    const data: NoteResponse = await response.json()
    noteStore.addNote(data.note)

    emit('note-created')
  } catch (err: unknown) {
    if (err instanceof Error) {
      errMessage.value = err.message
    }
  } finally {
    waiting.value = false
    resetForm()
  }
}

function resetForm() {
  title.value = ''
  content.value = ''
  inputErrors.value = {}
}
</script>

<template>
  <div class="mt-4">
    <p v-if="errMessage" class="text-red-500 text-sm">{{ errMessage }}</p>

    <form @submit.prevent="handleCreateNote" class="space-y-6">
      <div>
        <label for="title" class="block text-sm/6 font-medium text-gray-900">Title</label>
        <div class="mt-2">
          <input
            type="text"
            v-model="title"
            id="title"
            class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:text-sm/6"
          />
        </div>
        <div v-if="inputErrors.title" class="mt-1 text-red-500 text-sm">
          <span v-for="(error, index) in inputErrors.title" :key="index">{{ error }}</span>
        </div>
      </div>

      <div>
        <label for="content" class="block text-sm/6 font-medium text-gray-900">Content</label>
        <div class="mt-2">
          <textarea
            v-model="content"
            id="content"
            class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:text-sm/6"
          />
        </div>
        <div v-if="inputErrors.content" class="mt-1 text-red-500 text-sm">
          <span v-for="(error, index) in inputErrors.content" :key="index">{{ error }}</span>
        </div>
      </div>

      <div>
        <button
          type="submit"
          :disabled="waiting"
          class="cursor-pointer flex w-full justify-center rounded-md bg-teal-600 px-3 py-2 text-sm/6 font-semibold text-white shadow-xs hover:bg-teal-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-teal-600"
        >
          {{ waiting ? 'Creating...' : 'Create' }}
        </button>
      </div>
    </form>
  </div>
</template>
