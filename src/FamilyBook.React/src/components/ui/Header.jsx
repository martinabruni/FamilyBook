import { Image } from 'lucide-react'
import { ThemeToggle } from './ThemeToggle'

export const Header = () => {
  return (
    <header className="border-b">
      <div className="container mx-auto px-4 py-4 flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Image className="h-8 w-8" />
          <h1 className="text-2xl font-bold">Photo Gallery</h1>
        </div>
        <ThemeToggle />
      </div>
    </header>
  )
}
