<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useNoteStore, type Note } from '@/stores/note'
import { useAuthStore } from '@/stores/auth'

const API_URL = import.meta.env.VITE_API_BASE_URL

interface NoteListResponse {
  notes: Note[]
}

const noteStore = useNoteStore()
const auth = useAuthStore()
const waiting = ref(true)
const errMessage = ref<string | null>(null)
const emit = defineEmits<{
  (e: 'edit-note', noteId: number): void
}>()
const searchTerm = ref<string>('')
let searchDebounce: number | undefined

async function fetchNotes() {
  waiting.value = true
  errMessage.value = null

  try {
    const url = new URL(`${API_URL}/notes`)
    if (searchTerm.value) {
      url.searchParams.append('q', searchTerm.value)
    }

    const response = await fetch(url, {
      headers: {
        Authorization: `Bearer ${auth.token}`,
        'Content-Type': 'application/json',
      },
    })


    if (!response.ok) throw new Error('Failed to load notes')

    const data: NoteListResponse = await response.json()
    noteStore.setNotes(data.notes)
  } catch (err: unknown) {
    if (err instanceof Error) {
      errMessage.value = err.message
    }
  } finally {
    waiting.value = false
  }
}

onMounted(() => {
  fetchNotes()
})

watch(searchTerm, () => {
  clearTimeout(searchDebounce)
  searchDebounce = setTimeout(() => {
    fetchNotes()
  }, 500)
})

function editNote(note: Note) {
  emit('edit-note', note.id)
}
</script>

<template>
  <div class="max-w-5xl mx-auto p-6 mt-4">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-3xl font-bold text-gray-800">My Notes</h2>
    </div>

    <div class="my-4">
      <form @submit.prevent="fetchNotes">
        <div class="w-60">
          <div class="mt-2 grid grid-cols-1">
            <input
              type="text"
              v-model="searchTerm"
              placeholder="Search ..."
              aria-label="Search ..."
              class="col-start-1 row-start-1 block w-full rounded-md bg-white py-1.5 pr-10 pl-3 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-teal-600 sm:pr-9 sm:text-sm/6"
            />
            <svg
              class="pointer-events-none col-start-1 row-start-1 mr-3 size-5 self-center justify-self-end text-gray-400 size-4"
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
                d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z"
              ></path>
            </svg>
          </div>
        </div>
      </form>
    </div>

    <div v-if="waiting" class="flex justify-center py-10">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-teal-600"></div>
    </div>

    <div
      v-else-if="errMessage"
      class="bg-red-50 text-red-600 p-4 rounded-md mb-4 border border-red-200"
    >
      ErrMessage: {{ errMessage }}
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-if="noteStore.notes.length > 0"
        v-for="note in noteStore.notes"
        :key="note.id"
        class="bg-white p-5 rounded-lg shadow-sm border border-gray-200 hover:shadow-md transition-shadow duration-200 flex flex-col"
      >
        <div class="flex justify-between items-start mb-2">
          <h3 class="text-xl font-semibold text-gray-900 line-clamp-1" :title="note.title">
            {{ note.title }}
          </h3>
        </div>

        <div
          class="mt-auto pt-4 border-t border-gray-100 flex justify-between items-center text-xs text-gray-400"
        >
          <span>{{ new Date(note.createdAt).toLocaleDateString() }} {{ new Date(note.createdAt).toLocaleTimeString() }}</span>

          <div class="space-x-2">
            <button class="cursor-pointer text-teal-500 hover:text-teal-700 font-medium" @click="editNote(note)">
              Edit
            </button>
          </div>
        </div>
      </div>
      <div v-else class="col-span-full text-center text-gray-500 text-lg">
        No notes found.
      </div>
    </div>
  </div>
</template>
