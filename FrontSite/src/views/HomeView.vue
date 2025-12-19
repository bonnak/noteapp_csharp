<script setup lang="ts">
import { ref } from 'vue'
import NoteList from '@/components/NoteList.vue'
import NoteCreate from '@/components/NoteCreate.vue'
import NoteEdit from '@/components/NoteEdit.vue'

const modalNoteCreateOpened = ref(false)
const editingNoteId = ref<number | null>(null)

function openNoteCreateModal() {
  modalNoteCreateOpened.value = true
}

function closeNoteCreateModal() {
  modalNoteCreateOpened.value = false
}

const modalNoteEditOpened = ref(false)

function openNoteEditModal(noteId: number) {
  editingNoteId.value = noteId
  modalNoteEditOpened.value = true
}

function closeNoteEditModal() {
  editingNoteId.value = null
  modalNoteEditOpened.value = false
}
</script>

<template>
  <main>
    <NoteList @edit-note="openNoteEditModal" />

    <div class="flex px-6">
      <div class="flex-grow"></div>
      <button
        type="button"
        @click="openNoteCreateModal"
        class="cursor-pointer rounded-md bg-white px-2.5 py-2 text-center text-sm font-semibold text-teal-600 hover:text-white shadow-xs ring-1 ring-teal-500 ring-inset hover:bg-teal-500"
      >
        New Note
      </button>
    </div>

    <!-- Note Create Modal -->
    <div class="relative z-50" role="dialog" aria-modal="true">
      <div
        class="fixed flex items-center justify-center inset-0 w-screen overflow-y-auto pt-6 sm:pt-0 bg-black/30 z-1000"
        v-if="modalNoteCreateOpened"
      >
        <div class="relative p-6 min-w-100 bg-white rounded-lg shadow-xl w-[90%]">
          <div class="flex flex-col min-h-50 w-full h-full">
            <h2
              class="text-lg/6 font-semibold text-balance text-zinc-950 sm:text-base/6 dark:text-white pb-2"
            >
              New Note
            </h2>
            <div>
              <NoteCreate @note-created="closeNoteCreateModal" />
            </div>
          </div>
          <!-- Close button -->
          <button
            class="cursor-pointer absolute w-max rounded-full border-neutral-950/20 -right-[26px] -top-[28px] border-8"
            style="border-radius: 100%"
            aria-label="Close"
            @click="closeNoteCreateModal"
          >
            <div class="rounded-full bg-neutral-950 p-2 opacity-70">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                viewBox="0 0 24 24"
                fill="none"
                stroke="white"
                stroke-width="3"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-x h-3.5 w-3.5 md:h-5 md:w-5"
                aria-hidden="true"
              >
                <path d="M18 6 6 18"></path>
                <path d="m6 6 12 12"></path>
              </svg>
            </div>
          </button>
          <!-- End close button -->
        </div>
      </div>
    </div>
    <!-- End Note Create Modal -->

    <!-- Note Edit Modal -->
    <div class="relative z-50" role="dialog" aria-modal="true">
      <div
        class="fixed flex items-center justify-center inset-0 w-screen overflow-y-auto pt-6 sm:pt-0 bg-black/30 z-1000"
        v-if="modalNoteEditOpened"
      >
        <div class="relative p-6 min-w-100 bg-white rounded-lg shadow-xl w-[90%]">
          <div class="flex flex-col min-h-50 w-full h-full">
            <h2
              class="text-lg/6 font-semibold text-balance text-zinc-950 sm:text-base/6 dark:text-white pb-2"
            >
              Edit Note {{ editingNoteId }}
            </h2>
            <div>
              <NoteEdit :noteId="editingNoteId" @note-edited="closeNoteEditModal" />
            </div>
          </div>
          <!-- Close button -->
          <button
            class="cursor-pointer absolute w-max rounded-full border-neutral-950/20 -right-[26px] -top-[28px] border-8"
            style="border-radius: 100%"
            aria-label="Close"
            @click="closeNoteEditModal"
          >
            <div class="rounded-full bg-neutral-950 p-2 opacity-70">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                viewBox="0 0 24 24"
                fill="none"
                stroke="white"
                stroke-width="3"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-x h-3.5 w-3.5 md:h-5 md:w-5"
                aria-hidden="true"
              >
                <path d="M18 6 6 18"></path>
                <path d="m6 6 12 12"></path>
              </svg>
            </div>
          </button>
          <!-- End close button -->
        </div>
      </div>
    </div>
    <!-- End Note Edit Modal -->
  </main>
</template>
