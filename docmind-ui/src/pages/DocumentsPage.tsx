import { useState, useEffect, useRef, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { Upload, Trash2, FileText, RefreshCw, ChevronRight } from 'lucide-react'
import { api } from '../api/client'
import type { Document } from '../types'
import StatusBadge from '../components/StatusBadge'

function formatSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1024 / 1024).toFixed(1)} MB`
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleString()
}

export default function DocumentsPage() {
  const [documents, setDocuments] = useState<Document[]>([])
  const [uploading, setUploading] = useState(false)
  const [dragOver, setDragOver] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const fileRef = useRef<HTMLInputElement>(null)
  const navigate = useNavigate()

  const load = useCallback(async () => {
    try {
      const docs = await api.documents.getAll()
      setDocuments(docs)
    } catch {
      setError('Failed to load documents')
    }
  }, [])

  useEffect(() => {
    load()
    const interval = setInterval(load, 5000)
    return () => clearInterval(interval)
  }, [load])

  const upload = async (file: File) => {
    setUploading(true)
    setError(null)
    try {
      await api.documents.upload(file)
      await load()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Upload failed')
    } finally {
      setUploading(false)
    }
  }

  const handleFile = (file: File | undefined) => { if (file) upload(file) }

  const handleDelete = async (e: React.MouseEvent, id: string) => {
    e.stopPropagation()
    if (!confirm('Delete this document and all its chunks?')) return
    try {
      await api.documents.delete(id)
      setDocuments(prev => prev.filter(d => d.id !== id))
      setError(null)
    } catch {
      setError('Delete failed')
    }
  }

  return (
    <div className="flex flex-col h-screen" style={{ backgroundColor: '#f9f9f9' }}>
      <div className="bg-white px-6 py-4 flex items-center justify-between" style={{ borderBottom: '1px solid #eee' }}>
        <div>
          <h1 className="font-semibold text-black">Knowledge Base</h1>
          <p className="text-xs mt-0.5" style={{ color: '#888' }}>{documents.length} document{documents.length !== 1 ? 's' : ''} indexed</p>
        </div>
        <button onClick={load} className="transition-colors" style={{ color: '#aaa' }}
          onMouseEnter={e => (e.currentTarget.style.color = '#FF5F2D')}
          onMouseLeave={e => (e.currentTarget.style.color = '#aaa')}>
          <RefreshCw size={16} />
        </button>
      </div>

      <div className="flex-1 overflow-y-auto p-6 flex flex-col gap-5 max-w-4xl w-full mx-auto">
        {/* Upload zone */}
        <div
          onDragOver={e => { e.preventDefault(); setDragOver(true) }}
          onDragLeave={() => setDragOver(false)}
          onDrop={e => { e.preventDefault(); setDragOver(false); handleFile(e.dataTransfer.files[0]) }}
          onClick={() => fileRef.current?.click()}
          className="border-2 border-dashed rounded-2xl p-10 flex flex-col items-center gap-3 cursor-pointer transition-colors bg-white"
          style={{ borderColor: dragOver ? '#FF5F2D' : '#ddd', backgroundColor: dragOver ? '#fff5f2' : '#fff' }}
        >
          <div className="w-12 h-12 rounded-xl flex items-center justify-center" style={{ backgroundColor: '#fff5f2' }}>
            <Upload size={22} style={{ color: '#FF5F2D' }} />
          </div>
          <div className="text-center">
            <p className="font-medium text-black">{uploading ? 'Uploading...' : 'Drop a file or click to upload'}</p>
            <p className="text-sm mt-1" style={{ color: '#888' }}>Supports .txt and .md files</p>
          </div>
          <input ref={fileRef} type="file" accept=".txt,.md" className="hidden" onChange={e => handleFile(e.target.files?.[0])} />
        </div>

        {error && (
          <div className="text-sm px-4 py-3 rounded-xl" style={{ backgroundColor: '#fff0f0', border: '1px solid #fcc', color: '#cc0000' }}>
            {error}
          </div>
        )}

        {documents.length > 0 && (
          <div className="flex flex-col gap-2">
            {documents.map(doc => (
              <div
                key={doc.id}
                onClick={() => navigate(`/documents/${doc.id}`)}
                className="bg-white rounded-xl px-4 py-3.5 flex items-center gap-4 cursor-pointer transition-all"
                style={{ border: '1px solid #eee' }}
                onMouseEnter={e => (e.currentTarget.style.borderColor = '#FF5F2D')}
                onMouseLeave={e => (e.currentTarget.style.borderColor = '#eee')}
              >
                <div className="w-9 h-9 rounded-lg flex items-center justify-center shrink-0" style={{ backgroundColor: '#f5f5f5' }}>
                  <FileText size={17} style={{ color: '#666' }} />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="font-medium text-black text-sm truncate">{doc.fileName}</p>
                  <p className="text-xs mt-0.5" style={{ color: '#aaa' }}>{formatSize(doc.size)} · {formatDate(doc.uploadedAt)}</p>
                </div>
                <StatusBadge status={doc.status} />
                <button onClick={e => handleDelete(e, doc.id)} className="transition-colors ml-1" style={{ color: '#ccc' }}
                  onMouseEnter={e => (e.currentTarget.style.color = '#FF5F2D')}
                  onMouseLeave={e => (e.currentTarget.style.color = '#ccc')}>
                  <Trash2 size={15} />
                </button>
                <ChevronRight size={15} style={{ color: '#ccc' }} />
              </div>
            ))}
          </div>
        )}

        {documents.length === 0 && !uploading && (
          <p className="text-center text-sm py-8" style={{ color: '#aaa' }}>No documents yet. Upload one above to get started.</p>
        )}
      </div>
    </div>
  )
}
