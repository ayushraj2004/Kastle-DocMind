import ReactMarkdown from 'react-markdown'
import type { Message } from '../types'
import CitationCard from './CitationCard'
import { User, Clock, Zap } from 'lucide-react'
import logo from '../assets/logo.png'

export default function MessageBubble({ message }: { message: Message }) {
  const isUser = message.role === 'user'

  if (message.isLoading) {
    return (
      <div className="flex gap-3 items-start">
        <div className="w-8 h-8 rounded-full bg-white border flex items-center justify-center shrink-0 overflow-hidden" style={{ borderColor: '#eee' }}>
          <img src={logo} alt="DocMind" className="w-7 h-7 object-contain" />
        </div>
        <div className="bg-white border border-gray-200 rounded-2xl rounded-tl-sm px-4 py-3 shadow-sm">
          <div className="flex gap-1 items-center h-5">
            <span className="w-2 h-2 rounded-full animate-bounce [animation-delay:0ms]" style={{ backgroundColor: '#FF5F2D' }} />
            <span className="w-2 h-2 rounded-full animate-bounce [animation-delay:150ms]" style={{ backgroundColor: '#FF5F2D' }} />
            <span className="w-2 h-2 rounded-full animate-bounce [animation-delay:300ms]" style={{ backgroundColor: '#FF5F2D' }} />
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className={`flex gap-3 items-start ${isUser ? 'flex-row-reverse' : ''}`}>
      <div className={`w-9 h-9 rounded-full flex items-center justify-center shrink-0 overflow-hidden ${isUser ? '' : 'bg-white border'}`}
        style={isUser ? { backgroundColor: '#000000' } : { borderColor: '#eee' }}>
        {isUser
          ? <User size={16} className="text-white" />
          : <img src={logo} alt="DocMind" className="w-7 h-7 object-contain" />}
      </div>

      <div className={`max-w-[75%] flex flex-col gap-2 ${isUser ? 'items-end' : 'items-start'}`}>
        <div className={`px-4 py-3 rounded-2xl shadow-sm text-sm leading-relaxed ${isUser ? 'text-white rounded-tr-sm' : 'bg-white border border-gray-200 text-gray-800 rounded-tl-sm'}`}
          style={isUser ? { backgroundColor: '#FF5F2D' } : {}}>
          {isUser ? (
            <p>{message.content}</p>
          ) : (
            <div className="prose prose-sm max-w-none prose-p:my-1 prose-ul:my-1 prose-li:my-0">
              <ReactMarkdown>{message.content}</ReactMarkdown>
            </div>
          )}
        </div>

        {!isUser && message.citations && message.citations.length > 0 && (
          <div className="flex flex-col gap-1.5 w-full">
            {message.citations.map(c => (
              <CitationCard key={c.chunkId} citation={c} />
            ))}
          </div>
        )}

        {!isUser && message.latencyMs !== undefined && (
          <div className="flex gap-3 text-xs text-gray-400 px-1">
            <span className="flex items-center gap-1">
              <Clock size={11} /> {(message.latencyMs / 1000).toFixed(1)}s
            </span>
            {message.tokensUsed && (
              <span className="flex items-center gap-1">
                <Zap size={11} /> {message.tokensUsed.prompt + message.tokensUsed.completion} tokens
              </span>
            )}
          </div>
        )}
      </div>
    </div>
  )
}
