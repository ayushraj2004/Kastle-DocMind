const styles: Record<string, React.CSSProperties> = {
  Uploaded:   { backgroundColor: '#f0f0f0', color: '#333' },
  Processing: { backgroundColor: '#fff5f2', color: '#FF5F2D' },
  Indexed:    { backgroundColor: '#f0faf0', color: '#2d7a2d' },
  Failed:     { backgroundColor: '#fff0f0', color: '#cc0000' },
}

export default function StatusBadge({ status }: { status: string }) {
  return (
    <span className="px-2 py-0.5 rounded-full text-xs font-medium" style={styles[status] ?? { backgroundColor: '#f0f0f0', color: '#333' }}>
      {status}
    </span>
  )
}
