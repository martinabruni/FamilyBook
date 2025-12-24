// Configurazione degli album fotografici
// In futuro, questo sarà sostituito da una chiamata API che legge da Google Drive/OneDrive

export const GALLERY_CONFIG = {
  // URL base dove risiedono le foto (GitHub, blob storage, ecc.)
  baseUrl: '/photos',

  // Definizione degli album
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      description: 'Ricordi delle festività natalizie',
      coverImage: 'natale-2025/natale.png',
      photos: [
        { id: 1, filename: 'natale.png', alt: 'Il trio' },
        { id: 2, filename: 'natale-jones.png', alt: 'Nora Jones' },
      ]
    }
  ]
}

// Helper per costruire l'URL completo di una foto
export const getPhotoUrl = (albumId, filename) => {
  return `${GALLERY_CONFIG.baseUrl}/${albumId}/${filename}`
}

// Helper per ottenere tutte le foto di un album
export const getAlbumPhotos = (albumId) => {
  const album = GALLERY_CONFIG.albums.find(a => a.id === albumId)
  if (!album) return []

  return album.photos.map(photo => ({
    ...photo,
    url: getPhotoUrl(albumId, photo.filename)
  }))
}

// Helper per ottenere l'URL della cover di un album
export const getAlbumCoverUrl = (album) => {
  return `${GALLERY_CONFIG.baseUrl}/${album.coverImage}`
}
