# Photo Gallery

Galleria fotografica moderna e responsive costruita con React, Vite e Tailwind CSS (shadcn/ui).

## 🎨 Caratteristiche

- ✅ Design moderno e pulito
- 📱 Completamente responsive
- 🌓 Modalità chiaro/scuro
- 🖼️ Visualizzazione album organizzati
- 🔍 Lightbox per foto a schermo intero
- ⌨️ Navigazione con frecce tra le foto
- 🎯 Zero duplicazione di codice
- 🚀 Pronto per il deploy su Azure Static Web Apps

## 🚀 Quick Start

```bash
# Installa le dipendenze
npm install

# Avvia il server di sviluppo
npm run dev

# Build per produzione
npm run build
```

## 📁 Struttura del Progetto

```
photo-gallery/
├── src/
│   ├── components/
│   │   ├── ui/              # Componenti shadcn/ui riutilizzabili
│   │   ├── AlbumCard.jsx    # Card singolo album
│   │   ├── AlbumGrid.jsx    # Griglia di album
│   │   ├── PhotoGallery.jsx # Galleria foto con lightbox
│   │   ├── Header.jsx       # Header con logo e toggle tema
│   │   └── ThemeProvider.jsx # Provider per gestione tema
│   ├── config/
│   │   └── gallery.js       # Configurazione album (da sostituire con API)
│   ├── lib/
│   │   └── utils.js         # Utility functions
│   ├── App.jsx              # Componente principale
│   ├── main.jsx             # Entry point
│   └── index.css            # Stili globali e variabili tema
├── public/
│   └── photos/              # Cartella dove mettere le foto
│       ├── natale-2025/
│       ├── estate-2024/
│       └── compleanno-2024/
└── package.json
```

## 🔧 Configurazione Album

Attualmente gli album sono configurati in `src/config/gallery.js`. Modifica questo file per aggiungere/rimuovere album.

```javascript
export const GALLERY_CONFIG = {
  baseUrl: '/photos',
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      description: 'Ricordi delle festività natalizie',
      coverImage: 'natale-2025/cover.jpg',
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Albero di Natale' },
        // ... altre foto
      ]
    }
  ]
}
```

## 📸 Organizzazione Foto

Metti le tue foto nella cartella `public/photos/` seguendo questa struttura:

```
public/photos/
├── natale-2025/
│   ├── cover.jpg    # Immagine di copertina dell'album
│   ├── foto1.jpg
│   ├── foto2.jpg
│   └── ...
├── estate-2024/
│   ├── cover.jpg
│   └── ...
└── ...
```

## 🌐 Migrazione a Google Drive / OneDrive

### Opzione 1: Link Pubblici Diretti (Più Semplice)

1. **Google Drive:**
   ```javascript
   // Converti il link di condivisione in link diretto
   // Da: https://drive.google.com/file/d/FILE_ID/view
   // A: https://drive.google.com/uc?export=view&id=FILE_ID
   ```

2. **OneDrive:**
   ```javascript
   // Usa il link di download diretto
   // Aggiungi ?download=1 al link di condivisione
   ```

3. Modifica `gallery.js` per usare i nuovi URL:
   ```javascript
   export const GALLERY_CONFIG = {
     baseUrl: '', // Lascia vuoto se usi URL completi
     albums: [
       {
         id: 'natale-2025',
         name: 'Natale 2025',
         coverImage: 'https://drive.google.com/uc?export=view&id=YOUR_FILE_ID',
         photos: [
           { 
             id: 1, 
             filename: 'https://drive.google.com/uc?export=view&id=PHOTO_ID',
             alt: 'Foto 1' 
           }
         ]
       }
     ]
   }
   ```

### Opzione 2: API Backend (Più Scalabile)

Crea un backend serverless (Azure Function o AWS Lambda) che:

1. Si connette a Google Drive API o Microsoft Graph API
2. Legge la struttura delle cartelle
3. Genera un JSON con metadati e link alle foto
4. L'app React fa fetch di questo JSON

**Esempio con Azure Function:**

```javascript
// azure-function/GetGallery/index.js
const { google } = require('googleapis')

module.exports = async function (context, req) {
  const drive = google.drive({ version: 'v3', auth: 'YOUR_API_KEY' })
  
  // Lista cartelle (album)
  const albums = await drive.files.list({
    q: "mimeType='application/vnd.google-apps.folder'",
    fields: 'files(id, name)'
  })
  
  // Per ogni album, lista foto
  const galleryData = await Promise.all(
    albums.data.files.map(async (album) => {
      const photos = await drive.files.list({
        q: `'${album.id}' in parents and mimeType contains 'image/'`,
        fields: 'files(id, name, webContentLink)'
      })
      
      return {
        id: album.id,
        name: album.name,
        photos: photos.data.files.map(photo => ({
          id: photo.id,
          filename: photo.webContentLink,
          alt: photo.name
        }))
      }
    })
  )
  
  context.res = {
    body: { albums: galleryData }
  }
}
```

**Aggiorna l'app React:**

```javascript
// src/config/gallery.js
export const fetchGalleryConfig = async () => {
  const response = await fetch('https://your-function.azurewebsites.net/api/GetGallery')
  return await response.json()
}
```

```javascript
// src/App.jsx
import { useState, useEffect } from 'react'
import { fetchGalleryConfig } from './config/gallery'

function App() {
  const [albums, setAlbums] = useState([])
  
  useEffect(() => {
    fetchGalleryConfig().then(data => setAlbums(data.albums))
  }, [])
  
  // ... resto del codice
}
```

## 🚢 Deploy su Azure Static Web Apps

1. Build del progetto:
   ```bash
   npm run build
   ```

2. La cartella `dist/` contiene l'app pronta per il deploy

3. Crea una Static Web App su Azure Portal

4. Configura GitHub Actions o Azure DevOps per il deploy automatico

### File di configurazione Azure (staticwebapp.config.json)

```json
{
  "routes": [
    {
      "route": "/*",
      "serve": "/index.html",
      "statusCode": 200
    }
  ],
  "navigationFallback": {
    "rewrite": "/index.html"
  }
}
```

## 🎨 Personalizzazione Tema

Modifica i colori in `src/index.css`:

```css
:root {
  --primary: 222.2 47.4% 11.2%;
  --secondary: 210 40% 96.1%;
  /* ... altri colori */
}

.dark {
  --primary: 210 40% 98%;
  --secondary: 217.2 32.6% 17.5%;
  /* ... altri colori */
}
```

## 📦 Dipendenze Principali

- React 18
- Vite 5
- Tailwind CSS 3
- lucide-react (icone)
- class-variance-authority (stili componenti)

## 🔮 Prossimi Sviluppi

- [ ] Paginazione per album con molte foto
- [ ] Ricerca/filtro foto
- [ ] Upload foto (con autenticazione)
- [ ] Condivisione album
- [ ] Download foto
- [ ] Slideshow automatico
- [ ] Tag e categorizzazione
- [ ] Integrazione con Azure Blob Storage

## 📄 Licenza

MIT
