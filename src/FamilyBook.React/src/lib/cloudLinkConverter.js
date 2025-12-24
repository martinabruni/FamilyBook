/**
 * Utility per convertire link di condivisione OneDrive/Google Drive
 * in link diretti utilizzabili per visualizzare immagini
 */

/**
 * Converte un link di condivisione OneDrive in link diretto
 * @param {string} shareUrl - URL di condivisione OneDrive
 * @returns {string} - URL diretto all'immagine
 */
export const convertOneDriveLink = (shareUrl) => {
  // Metodo 1: Se è un link breve (1drv.ms)
  if (shareUrl.includes('1drv.ms')) {
    // Aggiungi parametro download se non c'è già
    if (!shareUrl.includes('download=1')) {
      const separator = shareUrl.includes('?') ? '&' : '?'
      return `${shareUrl}${separator}download=1`
    }
    return shareUrl
  }

  // Metodo 2: Se è un link embed
  if (shareUrl.includes('embed?')) {
    // Sostituisci 'embed?' con 'download?'
    return shareUrl.replace('embed?', 'download?')
  }

  // Metodo 3: Link normale onedrive.live.com
  if (shareUrl.includes('onedrive.live.com')) {
    // Aggiungi download=1
    const separator = shareUrl.includes('?') ? '&' : '?'
    return `${shareUrl}${separator}download=1`
  }

  // Se è già un link diretto (public.bn.files.1drv.com) restituiscilo così com'è
  if (shareUrl.includes('public.bn.files.1drv.com') || 
      shareUrl.includes('public.dm.files.1drv.com')) {
    return shareUrl
  }

  // Altrimenti restituisci l'URL originale
  return shareUrl
}

/**
 * Converte un link di condivisione Google Drive in link diretto
 * @param {string} shareUrl - URL di condivisione Google Drive
 * @returns {string} - URL diretto all'immagine
 */
export const convertGoogleDriveLink = (shareUrl) => {
  // Estrai l'ID del file dal link
  const patterns = [
    /\/file\/d\/([^/]+)/,           // https://drive.google.com/file/d/FILE_ID/view
    /id=([^&]+)/,                   // https://drive.google.com/open?id=FILE_ID
    /\/d\/([^/]+)/,                 // Altre varianti
  ]

  for (const pattern of patterns) {
    const match = shareUrl.match(pattern)
    if (match) {
      const fileId = match[1]
      return `https://drive.google.com/uc?export=view&id=${fileId}`
    }
  }

  // Se è già un link diretto, restituiscilo
  if (shareUrl.includes('drive.google.com/uc?')) {
    return shareUrl
  }

  return shareUrl
}

/**
 * Rileva automaticamente il tipo di link e lo converte
 * @param {string} url - URL da convertire
 * @returns {string} - URL diretto
 */
export const convertToDirectLink = (url) => {
  if (!url) return url

  // OneDrive
  if (url.includes('1drv.ms') || 
      url.includes('onedrive.live.com') || 
      url.includes('sharepoint.com')) {
    return convertOneDriveLink(url)
  }

  // Google Drive
  if (url.includes('drive.google.com')) {
    return convertGoogleDriveLink(url)
  }

  // Altri servizi cloud potrebbero essere aggiunti qui
  // Dropbox, iCloud, etc.

  return url
}

/**
 * Verifica se un URL è un link diretto all'immagine
 * @param {string} url - URL da verificare
 * @returns {boolean}
 */
export const isDirectImageLink = (url) => {
  const directPatterns = [
    /public\.bn\.files\.1drv\.com/,           // OneDrive direct
    /public\.dm\.files\.1drv\.com/,           // OneDrive direct
    /drive\.google\.com\/uc\?/,               // Google Drive direct
    /\.(jpg|jpeg|png|gif|webp)$/i,            // Estensione immagine diretta
  ]

  return directPatterns.some(pattern => pattern.test(url))
}

/**
 * Esempio di utilizzo con la configurazione gallery
 */
export const processGalleryConfig = (config) => {
  return {
    ...config,
    albums: config.albums.map(album => ({
      ...album,
      coverImage: convertToDirectLink(album.coverImage),
      photos: album.photos.map(photo => ({
        ...photo,
        filename: convertToDirectLink(photo.filename)
      }))
    }))
  }
}
