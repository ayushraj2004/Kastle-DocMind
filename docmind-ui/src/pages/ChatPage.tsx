import { useState, useRef, useEffect } from 'react'
import { Send, Trash2 } from 'lucide-react'
import { api } from '../api/client'
import type { Message, ConversationTurn, Citation } from '../types'
import MessageBubble from '../components/MessageBubble'
import logo from '../assets/logo.png'

interface Props {
  messages: Message[]
  setMessages: React.Dispatch<React.SetStateAction<Message[]>>
}

export default function ChatPage({ messages, setMessages }: Props) {
  const [input, setInput] = useState('')
  const loading = messages.at(-1)?.isLoading ?? false
  const bottomRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages])

  const buildHistory = (): ConversationTurn[] =>
    messages.filter(m => !m.isLoading).map(m => ({ role: m.role, content: m.content }))

  const send = async () => {
    const question = input.trim()
    if (!question || loading) return

    const userMsg: Message = { id: crypto.randomUUID(), role: 'user', content: question }
    const loadingMsg: Message = { id: crypto.randomUUID(), role: 'assistant', content: '', isLoading: true }

    setMessages(prev => [...prev, userMsg, loadingMsg])
    setInput('')

    try {
      const res = await api.chat(question, buildHistory())

      const citations: Citation[] = res.citations?.length > 0
        ? res.citations
        : (res.retrievedChunks ?? []).map((chunk, i) => ({
            marker: i + 1,
            documentId: chunk.documentId,
            chunkId: chunk.chunkId,
            fileName: chunk.fileName,
            section: chunk.section,
          }))

      setMessages(prev => prev.map(m =>
        m.isLoading
          ? { ...m, content: res.answer, citations, retrievedChunks: res.retrievedChunks, tokensUsed: res.tokensUsed, latencyMs: res.latencyMs, isLoading: false }
          : m
      ))
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Something went wrong'
      setMessages(prev => prev.map(m =>
        m.isLoading ? { ...m, content: `Error: ${message}`, isLoading: false } : m
      ))
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); send() }
  }

  return (
    <div className="flex flex-col h-screen" style={{ backgroundColor: '#f9f9f9' }}>
      {/* Header */}
      <div className="bg-white px-6 py-4 flex items-center justify-between" style={{ borderBottom: '1px solid #eee' }}>
        <div>
          <h1 className="font-semibold text-black">Ask your documents</h1>
          <p className="text-xs mt-0.5" style={{ color: '#888' }}>Answers grounded in your uploaded knowledge base</p>
        </div>
        {messages.length > 0 && (
          <button onClick={() => setMessages([])} className="flex items-center gap-1.5 text-xs transition-colors" style={{ color: '#aaa' }}
            onMouseEnter={e => (e.currentTarget.style.color = '#FF5F2D')}
            onMouseLeave={e => (e.currentTarget.style.color = '#aaa')}>
            <Trash2 size={13} /> Clear chat
          </button>
        )}
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-6 py-6 flex flex-col gap-5">
        {messages.length === 0 && (
          <div className="flex flex-col items-center justify-center h-full gap-4 text-center">
            <img src={logo} alt="DocMind" className="h-20 w-auto opacity-90" />
            <p className="font-semibold text-black text-lg">Ask anything about your documents</p>
            <p className="text-sm max-w-sm" style={{ color: '#888' }}>
              Upload documents from the Documents page, then ask questions here. DocMind will find relevant sections and answer with citations.
            </p>
          </div>
        )}
        {messages.map(m => <MessageBubble key={m.id} message={m} />)}
        <div ref={bottomRef} />
      </div>

      {/* Input */}
      <div className="bg-white px-6 py-4" style={{ borderTop: '1px solid #eee' }}>
        <div className="flex gap-3 items-end max-w-4xl mx-auto">
          <textarea
            value={input}
            onChange={e => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Ask a question about your documents..."
            rows={1}
            className="flex-1 resize-none rounded-xl px-4 py-3 text-sm outline-none"
            style={{ border: '1px solid #ddd', maxHeight: '120px' }}
            onFocus={e => (e.currentTarget.style.borderColor = '#FF5F2D')}
            onBlur={e => (e.currentTarget.style.borderColor = '#ddd')}
          />
          <button
            onClick={send}
            disabled={!input.trim() || loading}
            className="w-11 h-11 rounded-xl flex items-center justify-center transition-colors shrink-0"
            style={{ backgroundColor: !input.trim() || loading ? '#eee' : '#FF5F2D' }}
          >
            <Send size={16} style={{ color: !input.trim() || loading ? '#aaa' : '#fff' }} />
          </button>
        </div>
        <p className="text-center text-xs mt-2" style={{ color: '#bbb' }}>Press Enter to send · Shift+Enter for new line</p>
      </div>
    </div>
  )
}
