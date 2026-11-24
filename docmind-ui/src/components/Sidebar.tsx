import { NavLink } from 'react-router-dom'
import { MessageSquare, FolderOpen } from 'lucide-react'
import logo from '../assets/logo.png'

const links = [
  { to: '/', icon: MessageSquare, label: 'Chat' },
  { to: '/documents', icon: FolderOpen, label: 'Documents' },
]

export default function Sidebar() {
  return (
    <aside className="w-60 shrink-0 flex flex-col h-screen sticky top-0" style={{ backgroundColor: '#FF5F2D' }}>
      <div className="flex items-center gap-2.5 px-5 py-5" style={{ borderBottom: '1px solid rgba(255,255,255,0.2)' }}>
        <img src={logo} alt="DocMind" className="h-16 w-auto" />
      </div>

      <nav className="flex flex-col gap-1 p-3 flex-1">
        {links.map(({ to, icon: Icon, label }) => (
          <NavLink
            key={to}
            to={to}
            end
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                isActive ? 'text-white' : 'text-white hover:text-white'
              }`
            }
            style={({ isActive }) => isActive ? { backgroundColor: '#000000' } : { opacity: 0.75 }}
          >
            <Icon size={17} />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="px-5 py-4 text-xs" style={{ borderTop: '1px solid rgba(255,255,255,0.2)', color: 'rgba(255,255,255,0.6)' }}>
        Powered by GlobalLogic · Ollama · Qdrant
      </div>
    </aside>
  )
}
