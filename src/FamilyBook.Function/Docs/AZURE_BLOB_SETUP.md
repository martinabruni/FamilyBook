# 🚀 Setup Azure Blob Storage per Photo Gallery

## Perché Azure Blob Storage?

✅ Nessun problema CORS
✅ Scalabile e professionale
✅ Integrazione perfetta con Azure Static Web Apps
✅ CDN opzionale per velocità
✅ Costa pochissimo (primi GB gratis)

---

## Setup Veloce (10 minuti)

### 1. Crea Storage Account

```bash
# Tramite Azure Portal o CLI
az storage account create \
  --name photogallerystorage \
  --resource-group mio-resource-group \
  --location westeurope \
  --sku Standard_LRS
```

### 2. Crea Container Pubblico

```bash
# Container per le foto
az storage container create \
  --name photos \
  --account-name photogallerystorage \
  --public-access blob
```

**Importante:** Imposta `--public-access blob` per permettere l'accesso pubblico alle immagini!

### 3. Carica le Foto

#### Opzione A: Azure Portal (UI)
1. Vai su portal.azure.com
2. Trova il tuo Storage Account
3. Containers → photos
4. Click "Upload"
5. Seleziona le foto

#### Opzione B: Azure CLI (Automatico)
```bash
# Carica tutte le foto di un album
az storage blob upload-batch \
  --account-name photogallerystorage \
  --destination photos/natale-2025 \
  --source ./local-photos/natale-2025 \
  --pattern "*.jpg"
```

#### Opzione C: Azure Storage Explorer (GUI)
1. Scarica [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/)
2. Connetti il tuo account
3. Drag & drop delle foto

### 4. Ottieni gli URL

Dopo l'upload, ogni foto avrà un URL tipo:
```
https://photogallerystorage.blob.core.windows.net/photos/natale-2025/foto1.jpg
```

### 5. Aggiorna la Configurazione

```javascript
// src/config/gallery.js
export const GALLERY_CONFIG = {
  baseUrl: 'https://photogallerystorage.blob.core.windows.net/photos',
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      description: 'Festività',
      coverImage: 'natale-2025/cover.jpg',
      photos: [
        { id: 1, filename: 'foto1.jpg', alt: 'Foto 1' },
        { id: 2, filename: 'foto2.jpg', alt: 'Foto 2' },
      ]
    }
  ]
}

// Gli URL completi saranno:
// https://photogallerystorage.blob.core.windows.net/photos/natale-2025/cover.jpg
// https://photogallerystorage.blob.core.windows.net/photos/natale-2025/foto1.jpg
```

---

## Abilitare CORS (Importante!)

Anche se il container è pubblico, potrebbe servire configurare CORS:

```bash
az storage cors add \
  --account-name photogallerystorage \
  --services b \
  --methods GET HEAD \
  --origins '*' \
  --allowed-headers '*' \
  --exposed-headers '*' \
  --max-age 3600
```

O tramite Portal:
1. Storage Account → Settings → Resource sharing (CORS)
2. Blob service → Add:
   - Allowed origins: `*`
   - Allowed methods: `GET`, `HEAD`
   - Allowed headers: `*`
   - Exposed headers: `*`
   - Max age: `3600`

---

## Script di Upload Automatico

Crea uno script per caricare facilmente nuove foto:

```bash
#!/bin/bash
# upload-to-azure.sh

STORAGE_ACCOUNT="photogallerystorage"
CONTAINER="photos"
LOCAL_DIR="./local-photos"

echo "📸 Caricamento foto su Azure Blob Storage..."

for album in "$LOCAL_DIR"/*; do
  if [ -d "$album" ]; then
    album_name=$(basename "$album")
    echo "📁 Caricamento album: $album_name"
    
    az storage blob upload-batch \
      --account-name $STORAGE_ACCOUNT \
      --destination $CONTAINER/$album_name \
      --source "$album" \
      --pattern "*.{jpg,jpeg,png}" \
      --overwrite
  fi
done

echo "✅ Caricamento completato!"
```

Usa con:
```bash
chmod +x upload-to-azure.sh
./upload-to-azure.sh
```

---

## Script per Generare Configurazione Automatica

```javascript
// scripts/generate-config-from-blob.js
const { BlobServiceClient } = require('@azure/storage-blob');
const fs = require('fs');

const ACCOUNT_NAME = 'photogallerystorage';
const CONTAINER_NAME = 'photos';
const CONNECTION_STRING = process.env.AZURE_STORAGE_CONNECTION_STRING;

async function generateConfig() {
  const blobServiceClient = BlobServiceClient.fromConnectionString(CONNECTION_STRING);
  const containerClient = blobServiceClient.getContainerClient(CONTAINER_NAME);

  const albums = {};

  // Lista tutti i blob
  for await (const blob of containerClient.listBlobsFlat()) {
    const parts = blob.name.split('/');
    if (parts.length !== 2) continue;

    const [albumId, filename] = parts;
    
    if (!albums[albumId]) {
      albums[albumId] = {
        id: albumId,
        name: albumId.split('-').map(w => 
          w.charAt(0).toUpperCase() + w.slice(1)
        ).join(' '),
        description: `Album fotografico`,
        coverImage: '',
        photos: []
      };
    }

    if (filename === 'cover.jpg' || filename === 'cover.png') {
      albums[albumId].coverImage = `${albumId}/${filename}`;
    } else if (/\.(jpg|jpeg|png|gif)$/i.test(filename)) {
      albums[albumId].photos.push({
        id: albums[albumId].photos.length + 1,
        filename: filename,
        alt: filename.replace(/\.(jpg|jpeg|png|gif)$/i, '')
      });
    }
  }

  const config = `
export const GALLERY_CONFIG = {
  baseUrl: 'https://${ACCOUNT_NAME}.blob.core.windows.net/${CONTAINER_NAME}',
  albums: ${JSON.stringify(Object.values(albums), null, 2)}
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
}

generateConfig();
```

Esegui con:
```bash
npm install @azure/storage-blob
export AZURE_STORAGE_CONNECTION_STRING="your-connection-string"
node scripts/generate-config-from-blob.js
```

---

## Costi

Azure Blob Storage è molto economico:

- **Primi 5GB**: Gratis (nel tier gratuito di Azure)
- **Storage**: ~€0.017/GB al mese
- **Transazioni**: Praticamente gratis per un sito personale

Esempio: 1000 foto (10GB) = ~€0.17/mese 💰

---

## CDN Opzionale (Per Performance)

Se vuoi velocità globale:

```bash
# Crea CDN endpoint
az cdn endpoint create \
  --resource-group mio-resource-group \
  --profile-name photo-gallery-cdn \
  --name photos \
  --origin photogallerystorage.blob.core.windows.net
```

URL diventa:
```
https://photos.azureedge.net/natale-2025/foto1.jpg
```

---

## Checklist Setup

- [ ] Storage Account creato
- [ ] Container pubblico creato (`--public-access blob`)
- [ ] CORS configurato per GET/HEAD
- [ ] Foto caricate nella struttura corretta
- [ ] URL testato nel browser (dovrebbe aprire l'immagine)
- [ ] Configurazione aggiornata con baseUrl corretto
- [ ] App testata in locale

---

## Troubleshooting

### Le foto non si vedono

1. Verifica che il container sia pubblico:
   ```bash
   az storage container show-permission \
     --name photos \
     --account-name photogallerystorage
   ```
   
   Deve mostrare: `"publicAccess": "blob"`

2. Verifica CORS:
   ```bash
   az storage cors list \
     --account-name photogallerystorage \
     --services b
   ```

3. Testa l'URL nel browser

### CORS ancora bloccato

Aggiungi il tuo dominio specifico invece di `*`:

```bash
az storage cors add \
  --account-name photogallerystorage \
  --services b \
  --methods GET HEAD \
  --origins 'http://localhost:5173' 'https://tuosito.azurestaticapps.net' \
  --allowed-headers '*' \
  --max-age 3600
```

---

## Bonus: Automazione completa

```yaml
# .github/workflows/upload-photos.yml
name: Upload Photos to Azure

on:
  push:
    paths:
      - 'photos/**'

jobs:
  upload:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Upload to Blob Storage
        run: |
          az storage blob upload-batch \
            --account-name photogallerystorage \
            --destination photos \
            --source ./photos \
            --overwrite
      
      - name: Generate Config
        run: |
          npm install
          node scripts/generate-config-from-blob.js
      
      - name: Commit Config
        run: |
          git config user.name "GitHub Actions"
          git add src/config/gallery.js
          git commit -m "Update gallery config" || true
          git push
```

Ora ogni volta che fai push di nuove foto, vengono automaticamente caricate su Azure e la configurazione viene aggiornata! 🚀
