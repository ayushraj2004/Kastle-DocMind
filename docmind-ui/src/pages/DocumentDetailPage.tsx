import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { ArrowLeft, Hash } from 'lucide-react'
import { api } from '../api/client'
import type { Document, Chunk } from '../types'
import StatusBadge from '../components/StatusBadge'

export default function DocumentDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [doc, setDoc] = useState<Document | null>(null)
  const [chunks, setChunks] = useState<Chunk[]>([])
  const [loading, setLoading] = useState(true)
  const [expanded, setExpanded] = useState<string | null>(null)

  useEffect(() => {
    if (!id) return
    Promise.all([api.documents.getById(id), api.documents.getChunks(id)])
      .then(([d, c]) => { setDoc(d); setChunks(c) })
      .finally(() => setLoading(false))
  }, [id])

  if (loading) return (
    <div className="flex items-center justify-center h-screen text-sm" style={{ color: '#aaa' }}>Loading...</div>
  )

  if (!doc) return (
    <div className="flex items-center justify-center h-screen text-sm" style={{ color: '#aaa' }}>Document not found.</div>
  )

  return (
    <div className="flex flex-col h-screen" style={{ backgroundColor: '#f9f9f9' }}>
      <div className="bg-white px-6 py-4 flex items-center gap-4" style={{ borderBottom: '1px solid #eee' }}>
        <button onClick={() => navigate('/documents')} className="transition-colors" style={{ color: '#aaa' }}
          onMouseEnter={e => (e.currentTarget.style.color = '#FF5F2D')}
          onMouseLeave={e => (e.currentTarget.style.color = '#aaa')}>
          <ArrowLeft size={18} />
        </button>
        <div className="flex-1 min-w-0">
          <h1 className="font-semibold text-black truncate">{doc.fileName}</h1>
          <p className="text-xs mt-0.5" style={{ color: '#aaa' }}>{chunks.length} chunks · {(doc.size / 1024).toFixed(1)} KB</p>
        </div>
        <StatusBadge status={doc.status} />
      </div>

      <div className="flex-1 overflow-y-auto p-6 max-w-4xl w-full mx-auto flex flex-col gap-3">
        {chunks.length === 0 && (
          <p className="text-center text-sm py-12" style={{ color: '#aaa' }}>
            {doc.status === 'Processing' ? 'Document is still being indexed...' : 'No chunks found for this document.'}
          </p>
        )}

        {chunks.map((chunk, i) => (
          <div key={chunk.chunkId} className="bg-white rounded-xl overflow-hidden" style={{ border: '1px solid #eee' }}>
            <button
              onClick={() => setExpanded(expanded === chunk.chunkId ? null : chunk.chunkId)}
              className="w-full flex items-center gap-3 px-4 py-3 text-left transition-colors"
              style={{ backgroundColor: expanded === chunk.chunkId ? '#fff5f2' : '#fff' }}
              onMouseEnter={e => { if (expanded !== chunk.chunkId) e.currentTarget.style.backgroundColor = '#fafafa' }}
              onMouseLeave={e => { if (expanded !== chunk.chunkId) e.currentTarget.style.backgroundColor = '#fff' }}
            >
              <span className="w-6 h-6 rounded-md text-white text-xs font-bold flex items-center justify-center shrink-0"
                style={{ backgroundColor: '#FF5F2D' }}>
                {i + 1}
              </span>
              <div className="flex-1 min-w-0">
                <p className="text-sm text-black truncate">{chunk.text.slice(0, 100)}...</p>
              </div>
              {chunk.section && (
                <span className="flex items-center gap-1 text-xs shrink-0" style={{ color: '#aaa' }}>
                  <Hash size={11} />{chunk.section}
                </span>
              )}
              {chunk.tokenCount > 0 && (
                <span className="text-xs shrink-0" style={{ color: '#ccc' }}>{chunk.tokenCount} tokens</span>
              )}
            </button>

            {expanded === chunk.chunkId && (
              <div className="px-4 pb-4" style={{ borderTop: '1px solid #f0f0f0' }}>
                <p className="text-sm leading-relaxed whitespace-pre-wrap pt-3" style={{ color: '#444' }}>{chunk.text}</p>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  )
}
