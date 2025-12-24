import { useState } from 'react'
import { Header } from './components/ui/Header'
import { AlbumGrid } from './components/ui/AlbumGrid'
import { PhotoGallery } from './components/ui/PhotoGallery'
import { ThemeProvider } from './components/ui/ThemeProvider'
import { GALLERY_CONFIG } from './config/gallery'

function App() {
  const [selectedAlbum, setSelectedAlbum] = useState(null)

  return (
    <ThemeProvider>
      <div className="min-h-screen bg-background">
        <Header />
        
        <main className="container mx-auto px-4 py-8">
          {selectedAlbum ? (
            <PhotoGallery 
              album={selectedAlbum} 
              onBack={() => setSelectedAlbum(null)}
            />
          ) : (
            <AlbumGrid 
              albums={GALLERY_CONFIG.albums}
              onAlbumClick={setSelectedAlbum}
            />
          )}
        </main>
      </div>
    </ThemeProvider>
  )
}

export default App
