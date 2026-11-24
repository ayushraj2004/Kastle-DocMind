import type { Citation } from '../types'
import { FileText } from 'lucide-react'

export default function CitationCard({ citation }: { citation: Citation }) {
  return (
    <div className="flex items-start gap-2 px-3 py-2 rounded-lg text-sm" style={{ backgroundColor: '#fff5f2', border: '1px solid #ffd5c8' }}>
      <span className="shrink-0 w-5 h-5 rounded-full text-white text-xs flex items-center justify-center font-bold" style={{ backgroundColor: '#FF5F2D' }}>
        {citation.marker}
      </span>
      <div className="flex items-center gap-1.5 min-w-0" style={{ color: '#c94a1f' }}>
        <FileText size={13} className="shrink-0" />
        <span className="font-medium truncate">{citation.fileName ?? 'Unknown file'}</span>
        {citation.section && (
          <span className="truncate opacity-70">— {citation.section}</span>
        )}
      </div>
    </div>
  )
}
