import { useState } from 'react'
import { X, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from './Button'
import { getAlbumPhotos } from '@/config/gallery'

export const PhotoGallery = ({ album, onBack }) => {
  const [selectedPhoto, setSelectedPhoto] = useState(null)
  const photos = getAlbumPhotos(album.id)

  const openLightbox = (photo, index) => {
    setSelectedPhoto({ ...photo, index })
  }

  const closeLightbox = () => {
    setSelectedPhoto(null)
  }

  const navigatePhoto = (direction) => {
    if (!selectedPhoto) return
    
    const newIndex = direction === 'next' 
      ? (selectedPhoto.index + 1) % photos.length
      : (selectedPhoto.index - 1 + photos.length) % photos.length
    
    setSelectedPhoto({ ...photos[newIndex], index: newIndex })
  }

  return (
    <>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <Button
              variant="ghost"
              onClick={onBack}
              className="mb-2"
            >
              <ChevronLeft className="h-4 w-4 mr-2" />
              Torna agli album
            </Button>
            <h2 className="text-3xl font-bold">{album.name}</h2>
            <p className="text-muted-foreground">{album.description}</p>
          </div>
        </div>

        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
          {photos.map((photo, index) => (
            <div
              key={photo.id}
              className="aspect-square overflow-hidden rounded-lg cursor-pointer hover:opacity-90 transition-opacity"
              onClick={() => openLightbox(photo, index)}
            >
              <img
                src={photo.url}
                alt={photo.alt}
                className="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
              />
            </div>
          ))}
        </div>
      </div>

      {selectedPhoto && (
        <div 
          className="fixed inset-0 bg-black/90 z-50 flex items-center justify-center"
          onClick={closeLightbox}
        >
          <Button
            variant="ghost"
            size="icon"
            className="absolute top-4 right-4 text-white hover:bg-white/20"
            onClick={closeLightbox}
          >
            <X className="h-6 w-6" />
          </Button>

          <Button
            variant="ghost"
            size="icon"
            className="absolute left-4 text-white hover:bg-white/20"
            onClick={(e) => {
              e.stopPropagation()
              navigatePhoto('prev')
            }}
          >
            <ChevronLeft className="h-8 w-8" />
          </Button>

          <Button
            variant="ghost"
            size="icon"
            className="absolute right-4 text-white hover:bg-white/20"
            onClick={(e) => {
              e.stopPropagation()
              navigatePhoto('next')
            }}
          >
            <ChevronRight className="h-8 w-8" />
          </Button>

          <img
            src={selectedPhoto.url}
            alt={selectedPhoto.alt}
            className="max-w-[90vw] max-h-[90vh] object-contain"
            onClick={(e) => e.stopPropagation()}
          />

          <div className="absolute bottom-8 left-1/2 -translate-x-1/2 text-white text-sm">
            {selectedPhoto.index + 1} / {photos.length}
          </div>
        </div>
      )}
    </>
  )
}
