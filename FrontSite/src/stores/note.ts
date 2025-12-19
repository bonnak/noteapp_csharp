import { ref } from 'vue'
import { defineStore } from 'pinia'

export interface Note {
  id: number
  title: string
  content?: string
  createdAt: string
  updatedAt: string
}

export const useNoteStore = defineStore('note', () => {
  const notes = ref<Note[]>([])

  function setNotes(newNotes: Note[]) {
    notes.value = newNotes
  }

  function addNote(newNote: Note) {
    notes.value = [newNote, ...notes.value]
  }

  return {
    notes,
    setNotes,
    addNote,
  }
})
