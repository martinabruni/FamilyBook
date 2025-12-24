# 🚀 Setup GitHub per Photo Gallery (Soluzione Più Semplice!)

## Perché GitHub?

✅ **Gratis** (fino a 1GB)
✅ **Zero problemi CORS**
✅ **Setup in 5 minuti**
✅ **Nessuna configurazione complicata**
✅ **Affidabile e veloce**

Perfetto per gallerie fotografiche personali o di medie dimensioni!

---

## Setup Veloce (5 Minuti)

### 1. Crea Repository

1. Vai su [github.com](https://github.com)
2. Click su **"New repository"**
3. Nome: `photo-gallery-images` (o quello che vuoi)
4. Visibilità: **Public** ⚠️ IMPORTANTE
5. Click **"Create repository"**

### 2. Carica le Foto

#### Opzione A: Web Interface (Più Semplice)

1. Nel repository, click **"Add file"** → **"Upload files"**
2. Crea la struttura:
   ```
   photos/
   ├── natale-2025/
   │   ├── cover.jpg
   │   ├── foto1.jpg
   │   └── foto2.jpg
   ├── estate-2024/
   │   └── ...
   ```
3. Drag & drop le tue cartelle
4. Click **"Commit changes"**

#### Opzione B: Git CLI (Più Veloce per Molte Foto)

```bash
# Clona il repository
git clone https://github.com/TUO_USERNAME/photo-gallery-images.git
cd photo-gallery-images

# Crea la struttura
mkdir -p photos/natale-2025 photos/estate-2024

# Copia le tue foto
cp -r ~/mie-foto/natale-2025/* photos/natale-2025/
cp -r ~/mie-foto/estate-2024/* photos/estate-2024/

# Commit e push
git add .
git commit -m "Add photos"
git push
```

### 3. Ottieni i Link Raw

Ogni foto avrà un URL tipo:
```
https://raw.githubusercontent.com/TUO_USERNAME/photo-gallery-images/main/photos/natale-2025/foto1.jpg
```

**Struttura URL:**
```
https://raw.githubusercontent.com/
    [username]/
    [repository]/
    [branch]/
    [path-to-file]
```

### 4. Aggiorna la Configurazione

```javascript
// src/config/gallery.js
export const GALLERY_CONFIG = {
  baseUrl: 'https://raw.githubusercontent.com/TUO_USERNAME/photo-gallery-images/main/photos',
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      description: 'Festività natalizie',
      coverImage: 'natale-2025/cover.jpg',
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
        { id: 1, filename: 'foto1.jpg', alt: 'Tramonto' },
        { id: 2, filename: 'foto2.jpg', alt: 'Spiaggia' },
      ]
    }
  ]
}
```

### 5. Testa!

```bash
npm run dev
```

Le foto dovrebbero apparire immediatamente! 🎉

---

## Script di Upload Automatico

Crea uno script per aggiungere facilmente nuove foto:

```bash
#!/bin/bash
# upload-photos-github.sh

REPO_DIR="./photo-gallery-images"
LOCAL_PHOTOS="./local-photos"

cd "$REPO_DIR"

# Pull delle ultime modifiche
git pull

# Copia le nuove foto
cp -r "$LOCAL_PHOTOS"/* photos/

# Commit e push
git add photos/
git commit -m "Add new photos $(date +%Y-%m-%d)"
git push

cd ..

echo "✅ Foto caricate su GitHub!"
```

Usa con:
```bash
chmod +x upload-photos-github.sh
./upload-photos-github.sh
```

---

## Script per Generare Configurazione Automatica

```javascript
// scripts/generate-config-from-github.js
const fs = require('fs');
const path = require('path');

const GITHUB_USERNAME = 'TUO_USERNAME';
const GITHUB_REPO = 'photo-gallery-images';
const GITHUB_BRANCH = 'main';
const PHOTOS_DIR = './photo-gallery-images/photos';

function generateConfig() {
  const baseUrl = `https://raw.githubusercontent.com/${GITHUB_USERNAME}/${GITHUB_REPO}/${GITHUB_BRANCH}/photos`;
  const albums = [];

  // Leggi tutte le cartelle (album)
  const albumDirs = fs.readdirSync(PHOTOS_DIR)
    .filter(file => fs.statSync(path.join(PHOTOS_DIR, file)).isDirectory());

  albumDirs.forEach(albumId => {
    const albumPath = path.join(PHOTOS_DIR, albumId);
    const files = fs.readdirSync(albumPath)
      .filter(file => /\.(jpg|jpeg|png|gif)$/i.test(file));

    // Trova la cover
    const coverFile = files.find(f => f.toLowerCase().includes('cover')) || files[0];
    
    // Lista foto (esclusa la cover)
    const photoFiles = files.filter(f => f !== coverFile);

    albums.push({
      id: albumId,
      name: albumId
        .split('-')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' '),
      description: `Album con ${photoFiles.length} foto`,
      coverImage: `${albumId}/${coverFile}`,
      photos: photoFiles.map((file, idx) => ({
        id: idx + 1,
        filename: file,
        alt: file.replace(/\.(jpg|jpeg|png|gif)$/i, '')
      }))
    });
  });

  const config = `
export const GALLERY_CONFIG = {
  baseUrl: '${baseUrl}',
  albums: ${JSON.stringify(albums, null, 2)}
}

export const getPhotoUrl = (albumId, filename) => {
  if (filename.startsWith('http://') || filename.startsWith('https://')) {
    return filename
  }
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
  if (album.coverImage.startsWith('http://') || album.coverImage.startsWith('https://')) {
    return album.coverImage
  }
  return \`\${GALLERY_CONFIG.baseUrl}/\${album.coverImage}\`
}
`;

  fs.writeFileSync('./src/config/gallery.js', config);
  console.log('✅ Configurazione generata!');
  console.log(`📁 ${albums.length} album trovati`);
  console.log(`📸 ${albums.reduce((sum, a) => sum + a.photos.length, 0)} foto totali`);
}

generateConfig();
```

**Configurazione:**
1. Clona sia il repo dell'app che quello delle foto:
   ```bash
   git clone https://github.com/TUO_USERNAME/photo-gallery.git
   git clone https://github.com/TUO_USERNAME/photo-gallery-images.git
   ```

2. Modifica lo script con il tuo username

3. Esegui:
   ```bash
   node scripts/generate-config-from-github.js
   ```

---

## Workflow Completo

### Aggiungere Nuove Foto

1. **Aggiungi foto al repo GitHub:**
   ```bash
   cd photo-gallery-images
   
   # Crea nuovo album
   mkdir -p photos/nuovo-album-2025
   cp ~/mie-foto/* photos/nuovo-album-2025/
   
   git add .
   git commit -m "Add nuovo-album-2025"
   git push
   ```

2. **Rigenera la configurazione:**
   ```bash
   cd ../photo-gallery
   node scripts/generate-config-from-github.js
   
   git add src/config/gallery.js
   git commit -m "Update gallery config"
   git push
   ```

3. **Deploy automatico** (se hai GitHub Actions configurato)

---

## Automazione con GitHub Actions

### Nel Repository delle Foto

Crea `.github/workflows/notify-update.yml`:

```yaml
name: Notify Photo Update

on:
  push:
    paths:
      - 'photos/**'

jobs:
  notify:
    runs-on: ubuntu-latest
    steps:
      - name: Trigger App Update
        run: |
          curl -X POST \
            -H "Authorization: token ${{ secrets.GALLERY_REPO_TOKEN }}" \
            -H "Accept: application/vnd.github.v3+json" \
            https://api.github.com/repos/TUO_USERNAME/photo-gallery/dispatches \
            -d '{"event_type":"photos_updated"}'
```

### Nel Repository dell'App

Crea `.github/workflows/update-config.yml`:

```yaml
name: Update Gallery Config

on:
  repository_dispatch:
    types: [photos_updated]
  workflow_dispatch:

jobs:
  update:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Clone Photos Repo
        run: |
          git clone https://github.com/TUO_USERNAME/photo-gallery-images.git
      
      - name: Setup Node
        uses: actions/setup-node@v2
        with:
          node-version: '18'
      
      - name: Generate Config
        run: |
          node scripts/generate-config-from-github.js
      
      - name: Commit Config
        run: |
          git config user.name "GitHub Actions"
          git config user.email "actions@github.com"
          git add src/config/gallery.js
          git commit -m "Auto-update gallery config" || echo "No changes"
          git push
```

Ora quando aggiungi foto al repo delle immagini, la configurazione dell'app si aggiorna automaticamente! 🤖

---

## Ottimizzazione Immagini

GitHub ha un limite di 100MB per file. Ottimizza le foto prima di caricarle:

```bash
# Usando ImageMagick
for img in photos/**/*.{jpg,jpeg}; do
  convert "$img" -resize "2000x2000>" -quality 85 "$img"
done

# Usando sharp (Node.js)
npm install -g sharp-cli
sharp -i input.jpg -o output.jpg --resize 2000 --quality 85
```

---

## Limiti e Considerazioni

### Limiti GitHub
- **Repository size**: Max 1GB (perfetto per ~1000-2000 foto ottimizzate)
- **File size**: Max 100MB per file
- **Bandwidth**: 100GB/mese (sufficiente per la maggior parte dei siti personali)

### Se superi i limiti
1. **Git LFS** (Large File Storage): estende i limiti
2. **Multiple repos**: crea più repository
3. **Azure Blob**: passa a storage dedicato

---

## Vantaggi vs OneDrive

| Feature | GitHub | OneDrive |
|---------|--------|----------|
| CORS | ✅ Nessun problema | ❌ Bloccato |
| Setup | ⚡ 5 minuti | 😰 Complicato |
| Costo | 🆓 Gratis | 💰 Richiede account |
| Performance | 🚀 CDN globale | 🐌 Medio |
| Limite | 1GB gratis | 5GB gratis, ma non usabile |
| Affidabilità | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |

---

## Checklist Setup

- [ ] Repository GitHub creato (PUBBLICO!)
- [ ] Foto caricate nella struttura corretta
- [ ] Link raw testato nel browser
- [ ] `baseUrl` configurato in `gallery.js`
- [ ] App testata in locale
- [ ] Deploy fatto su Azure Static Web Apps

---

## FAQ

**Q: Le foto sono private su GitHub?**
A: No, il repository deve essere PUBBLICO per usare raw.githubusercontent.com. Se vuoi privacy assoluta, usa Azure Blob Storage privato con SAS token.

**Q: Posso usare GitHub Pages invece di raw.githubusercontent.com?**
A: Sì! Abilita GitHub Pages sul repo delle foto, poi usa:
```
https://TUO_USERNAME.github.io/photo-gallery-images/photos/album/foto.jpg
```

**Q: E se supero 1GB?**
A: 
1. Ottimizza meglio le foto (WebP invece di JPEG)
2. Usa Git LFS
3. Crea un secondo repository
4. Passa ad Azure Blob Storage

**Q: Posso mescolare GitHub e foto locali?**
A: Sì! L'app supporta mix:
```javascript
albums: [
  { 
    photos: [
      { filename: 'https://raw.githubusercontent.com/.../foto1.jpg' }, // GitHub
      { filename: 'foto2.jpg' } // Locale
    ]
  }
]
```

---

## 🎉 TL;DR (Procedura Velocissima)

1. Crea repo pubblico su GitHub
2. Carica foto in `photos/album-name/`
3. Aggiorna `gallery.js`:
   ```javascript
   baseUrl: 'https://raw.githubusercontent.com/USERNAME/REPO/main/photos'
   ```
4. Fatto! ✨
