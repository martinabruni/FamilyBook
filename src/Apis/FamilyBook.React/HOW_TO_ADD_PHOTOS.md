# Come Aggiungere le Tue Foto

## Struttura Cartelle

Posiziona le tue foto nella cartella `public/photos/` seguendo questa struttura:

```
public/photos/
├── natale-2025/           # Nome album (diventerà "Natale 2025" nell'app)
│   ├── cover.jpg          # OBBLIGATORIO - Immagine di copertina dell'album
│   ├── foto1.jpg          # Le tue foto
│   ├── foto2.jpg
│   ├── foto3.jpg
│   └── ...
├── estate-2024/
│   ├── cover.jpg
│   ├── foto1.jpg
│   └── ...
└── compleanno-2024/
    ├── cover.jpg
    └── ...
```

## Formati Supportati

- ✅ JPG / JPEG
- ✅ PNG
- ✅ GIF
- ✅ WEBP

## Convenzioni di Naming

1. **Nome Cartella**: Usa il formato `nome-anno` (es. `natale-2025`, `estate-2024`)
   - Usa solo lettere minuscole
   - Usa trattini `-` al posto degli spazi
   - L'app convertirà automaticamente in formato leggibile

2. **Cover Image**: Ogni album DEVE avere un file chiamato `cover.jpg` (o .png)
   - Questo sarà mostrato come anteprima dell'album
   - Dimensioni consigliate: 1200x1200px (quadrata)

3. **Nome Foto**: Puoi usare qualsiasi nome
   - Consiglio: `foto1.jpg`, `foto2.jpg`, ecc. per semplicità
   - Oppure nomi descrittivi: `tramonto.jpg`, `famiglia.jpg`, ecc.

## Configurazione Album

Dopo aver aggiunto le cartelle, aggiorna il file `src/config/gallery.js`:

```javascript
export const GALLERY_CONFIG = {
  baseUrl: '/photos',
  albums: [
    {
      id: 'natale-2025',              // DEVE corrispondere al nome cartella
      name: 'Natale 2025',             // Nome visualizzato nell'app
      description: 'Festività natalizie', // Breve descrizione
      coverImage: 'natale-2025/cover.jpg', // Path alla cover
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Albero di Natale' },
        { id: 2, filename: 'foto2.jpg', alt: 'Cenone' },
        { id: 3, filename: 'foto3.jpg', alt: 'Regali' },
        // Aggiungi tutte le tue foto qui
      ]
    },
    // Aggiungi altri album...
  ]
}
```

## Ottimizzazione Immagini

Per prestazioni migliori, ottimizza le immagini prima di caricarle:

### Dimensioni Consigliate

- **Cover**: 1200x1200px (massimo 500KB)
- **Foto normali**: Larghezza massima 2000px (massimo 1MB)
- **Thumbnail** (opzionale): 400x400px (massimo 100KB)

### Strumenti Consigliati

- **Online**: [TinyPNG](https://tinypng.com), [Squoosh](https://squoosh.app)
- **Desktop**: [ImageOptim](https://imageoptim.com) (Mac), [RIOT](https://riot-optimizer.com) (Windows)
- **CLI**: `npm install -g sharp-cli` poi `sharp -i input.jpg -o output.jpg --resize 2000`

### Script di Ottimizzazione Automatica

Puoi creare uno script per ottimizzare tutte le immagini:

```bash
#!/bin/bash
# optimize-images.sh

for img in public/photos/**/*.{jpg,jpeg,png}; do
  echo "Optimizing $img..."
  # Usando ImageMagick
  convert "$img" -resize "2000x2000>" -quality 85 "$img"
done
```

## Generazione Automatica Configurazione

Se hai molte foto, puoi usare questo script Node.js per generare automaticamente la configurazione:

```javascript
// scripts/generate-config.js
const fs = require('fs');
const path = require('path');

const photosDir = path.join(__dirname, '../public/photos');
const albums = [];

// Leggi tutte le cartelle
fs.readdirSync(photosDir).forEach(albumFolder => {
  const albumPath = path.join(photosDir, albumFolder);
  
  if (fs.statSync(albumPath).isDirectory()) {
    const photos = fs.readdirSync(albumPath)
      .filter(file => /\.(jpg|jpeg|png|gif)$/i.test(file))
      .filter(file => file !== 'cover.jpg')
      .map((file, idx) => ({
        id: idx + 1,
        filename: file,
        alt: file.replace(/\.(jpg|jpeg|png|gif)$/i, '')
      }));

    albums.push({
      id: albumFolder,
      name: albumFolder
        .split('-')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' '),
      description: `Album con ${photos.length} foto`,
      coverImage: `${albumFolder}/cover.jpg`,
      photos
    });
  }
});

const config = `
export const GALLERY_CONFIG = {
  baseUrl: '/photos',
  albums: ${JSON.stringify(albums, null, 2)}
}

export const getPhotoUrl = (albumId, filename) => {
  return \`\${GALLERY_CONFIG.baseUrl}/\${albumId}/\${filename}\`
}

export const getAlbumPhotos = (albumId) => {
  const album = GALLERY_CONFIG.albums.find(a => a.id === albumId)
  if (!album) return []
  
  return album.photos.map(photo => ({
    ...photo,
    url: getPhotoUrl(albumId, photo.filename)
  }))
}

export const getAlbumCoverUrl = (album) => {
  return \`\${GALLERY_CONFIG.baseUrl}/\${album.coverImage}\`
}
`;

fs.writeFileSync(
  path.join(__dirname, '../src/config/gallery.js'),
  config
);

console.log('✅ Configurazione generata con successo!');
console.log(`📁 ${albums.length} album trovati`);
```

Esegui con: `node scripts/generate-config.js`

## Checklist Finale

Prima di fare il deploy, verifica:

- [ ] Ogni album ha un file `cover.jpg`
- [ ] Tutte le immagini sono ottimizzate (< 1MB)
- [ ] Il file `src/config/gallery.js` è aggiornato
- [ ] Hai testato l'app in locale con `npm run dev`
- [ ] Le foto si caricano correttamente
- [ ] Il tema dark/light funziona bene con le tue foto

## Troubleshooting

### Le foto non si vedono

1. Controlla che i nomi file in `gallery.js` corrispondano esattamente ai file
2. Verifica che le foto siano nella cartella `public/photos/`
3. Controlla la console del browser per errori 404

### Le foto sono troppo lente

1. Ottimizza le dimensioni (vedi sopra)
2. Considera di usare WebP invece di JPG
3. Valuta l'uso di un CDN per immagini (Cloudinary, ImageKit, ecc.)

### Voglio aggiungere molti album

Usa lo script di generazione automatica (vedi sopra) invece di modificare manualmente `gallery.js`
