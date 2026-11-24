import type { ChatResponse, ConversationTurn, Document, Chunk } from '../types'

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, options)
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: res.statusText }))
    throw new Error(err.message ?? res.statusText)
  }
  if (res.status === 204 || res.headers.get('content-length') === '0') return undefined as T
  return res.json()
}

export const api = {
  chat: (question: string, history: ConversationTurn[]): Promise<ChatResponse> =>
    request('/chat', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        question,
        conversationHistory: history.length > 0 ? history : undefined,
      }),
    }),

  documents: {
    getAll: (page = 1, pageSize = 20): Promise<Document[]> =>
      request(`/documents?page=${page}&pageSize=${pageSize}`),

    getById: (id: string): Promise<Document> =>
      request(`/documents/${id}`),

    getChunks: (id: string): Promise<Chunk[]> =>
      request(`/documents/${id}/chunks`),

    upload: (file: File): Promise<Document> => {
      const form = new FormData()
      form.append('file', file)
      return request('/documents', { method: 'POST', body: form })
    },

    delete: (id: string): Promise<void> =>
      request(`/documents/${id}`, { method: 'DELETE' }),
  },
}
