// ESEMPIO DI CONFIGURAZIONE CON GITHUB
// Copia questo file in src/config/gallery.js e sostituisci con i tuoi dati

import { convertToDirectLink } from '../lib/cloudLinkConverter'

export const GALLERY_CONFIG = {
  // ⚠️ IMPORTANTE: Sostituisci con il tuo username e repository GitHub
  baseUrl: 'https://raw.githubusercontent.com/TUO_USERNAME/photo-gallery-images/main/photos',
  
  // Esempio di struttura album
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      description: 'Festività natalizie',
      // Cover dell'album (relativa a baseUrl)
      coverImage: 'natale-2025/cover.jpg',
      // URL completo sarà: https://raw.githubusercontent.com/TUO_USERNAME/photo-gallery-images/main/photos/natale-2025/cover.jpg
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Albero di Natale' },
        { id: 2, filename: 'foto2.jpg', alt: 'Cenone' },
        { id: 3, filename: 'foto3.jpg', alt: 'Regali' },
      ]
    },
    {
      id: 'estate-2024',
      name: 'Estate 2024',
      description: 'Vacanze al mare',
      coverImage: 'estate-2024/cover.jpg',
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Tramonto sul mare' },
        { id: 2, filename: 'foto2.jpg', alt: 'Spiaggia' },
      ]
    }
  ]
}

// ALTERNATIVA: Se vuoi usare Azure Blob Storage invece di GitHub
// Decommentare e modificare:
/*
export const GALLERY_CONFIG = {
  baseUrl: 'https://tuostorage.blob.core.windows.net/photos',
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      coverImage: 'natale-2025/cover.jpg',
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Foto 1' }
      ]
    }
  ]
}
*/

// ALTERNATIVA: Mix di fonti (GitHub + Locale + Azure)
/*
export const GALLERY_CONFIG = {
  baseUrl: '/photos', // Default per foto locali
  albums: [
    {
      id: 'album-github',
      name: 'Album da GitHub',
      // URL completo - ignorerà baseUrl
      coverImage: 'https://raw.githubusercontent.com/user/repo/main/photos/cover.jpg',
      photos: [
        { 
          id: 1, 
          // Foto da GitHub (URL completo)
          filename: 'https://raw.githubusercontent.com/user/repo/main/photos/foto1.jpg',
          alt: 'Foto 1' 
        }
      ]
    },
    {
      id: 'album-locale',
      name: 'Album Locale',
      // Path relativo - userà baseUrl
      coverImage: 'album-locale/cover.jpg',
      photos: [
        { 
          id: 1, 
          // Foto locale (userà baseUrl: /photos/album-locale/foto1.jpg)
          filename: 'foto1.jpg',
          alt: 'Foto 1' 
        }
      ]
    }
  ]
}
*/

// Helper per costruire l'URL completo di una foto
export const getPhotoUrl = (albumId, filename) => {
  // Se il filename è già un URL completo (GitHub/Azure/ecc.)
  if (filename.startsWith('http://') || filename.startsWith('https://')) {
    return convertToDirectLink(filename)
  }
  
  // Altrimenti usa il baseUrl
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
  // Se coverImage è già un URL completo
  if (album.coverImage.startsWith('http://') || album.coverImage.startsWith('https://')) {
    return convertToDirectLink(album.coverImage)
  }
  
  // Altrimenti usa il baseUrl
  return `${GALLERY_CONFIG.baseUrl}/${album.coverImage}`
}
