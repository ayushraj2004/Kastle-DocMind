export interface Document {
  id: string
  fileName: string
  size: number
  contentType: string
  uploadedAt: string
  status: 'Uploaded' | 'Processing' | 'Indexed' | 'Failed'
  filePath: string
}

export interface Citation {
  marker: number
  documentId: string
  chunkId: string
  fileName: string | null
  section: string | null
}

export interface RetrievedChunk {
  chunkId: string
  documentId: string
  fileName: string | null
  section: string | null
  text: string
  score: number
}

export interface TokenUsage {
  prompt: number
  completion: number
}

export interface ChatResponse {
  answer: string
  citations: Citation[]
  retrievedChunks: RetrievedChunk[]
  tokensUsed: TokenUsage
  latencyMs: number
}

export interface ConversationTurn {
  role: 'user' | 'assistant'
  content: string
}

export interface Message {
  id: string
  role: 'user' | 'assistant'
  content: string
  citations?: Citation[]
  retrievedChunks?: RetrievedChunk[]
  tokensUsed?: TokenUsage
  latencyMs?: number
  isLoading?: boolean
}

export interface Chunk {
  chunkId: string
  documentId: string
  text: string
  section: string | null
  sequenceNumber: number
  tokenCount: number
}
