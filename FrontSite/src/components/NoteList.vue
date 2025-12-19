<script setup lang="ts">
import { onMounted, ref } from 'vue'
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

async function fetchNotes() {
  waiting.value = true
  errMessage.value = null

  try {
    const response = await fetch(`${API_URL}/notes`, {
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

function editNote(note: Note) {
  emit('edit-note', note.id)
}
</script>

<template>
  <div class="max-w-5xl mx-auto p-6 mt-4">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-3xl font-bold text-gray-800">My Notes</h2>
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
      <div
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
          <span>{{ new Date(note.createdAt).toLocaleDateString() }}</span>

          <div class="space-x-2">
            <button class="text-teal-500 hover:text-teal-700 font-medium" @click="editNote(note)">
              Edit
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
