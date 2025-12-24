# 🔗 Come Ottenere Link OneDrive Funzionanti

## Il Problema

OneDrive ti dà link di condivisione tipo:
- `https://1drv.ms/i/s!AjxK...` (link breve)
- `https://onedrive.live.com/?cid=xxx&id=xxx` (link lungo)

Questi **NON** sono link diretti alle immagini, ma puntano a pagine HTML di visualizzazione.

---

## ✅ Soluzione 1: Link Embed (CONSIGLIATO)

Questo metodo ottiene il link diretto all'immagine.

### Passo 1: Condividi il file
1. Vai su OneDrive web (onedrive.live.com)
2. Trova la tua foto
3. Click destro → **Incorpora** (o "Embed")

### Passo 2: Copia il link
OneDrive ti mostrerà un codice HTML simile a questo:

```html
<img src="https://public.bn.files.1drv.com/y4m.../foto.jpg" 
     width="400" height="300" />
```

### Passo 3: Usa il link
Copia **SOLO** l'URL dentro `src="..."`:
```
https://public.bn.files.1drv.com/y4m.../foto.jpg
```

### Passo 4: Metti nella configurazione

```javascript
// src/config/gallery.js
albums: [
  {
    id: 'mio-album',
    name: 'Mio Album',
    coverImage: 'https://public.bn.files.1drv.com/y4m.../cover.jpg', // ✅ Link diretto
    photos: [
      { 
        id: 1, 
        filename: 'https://public.bn.files.1drv.com/y4m.../foto1.jpg', // ✅ Link diretto
        alt: 'Foto 1' 
      }
    ]
  }
]
```

---

## ✅ Soluzione 2: Link Download (PIÙ SEMPLICE, ma meno affidabile)

Se il metodo Embed non funziona, prova questo:

### Passo 1: Ottieni link di condivisione
1. Click destro sulla foto → **Condividi**
2. Copia il link (es. `https://1drv.ms/i/s!xxx`)

### Passo 2: Aggiungi parametro download
Aggiungi `?download=1` alla fine del link:

```
https://1drv.ms/i/s!xxx?download=1
```

### Passo 3: Usa nella configurazione

```javascript
albums: [
  {
    coverImage: 'https://1drv.ms/i/s!xxx?download=1',
    photos: [
      { id: 1, filename: 'https://1drv.ms/i/s!yyy?download=1', alt: 'Foto' }
    ]
  }
]
```

**NOTA:** L'app convertirà automaticamente i link grazie alla funzione `convertToDirectLink()`, quindi anche se dimentichi `?download=1`, verrà aggiunto automaticamente!

---

## ✅ Soluzione 3: Condivisione Cartella (Per molte foto)

Invece di condividere ogni foto singolarmente:

### Metodo A: Screenshot e Re-upload (Temporaneo)
1. Apri la foto su OneDrive
2. Click destro → Apri in nuova scheda
3. Quando si apre, fai click destro sull'immagine → **Copia indirizzo immagine**
4. Usa quell'URL (sarà un link diretto tipo `public.bn.files.1drv.com`)

### Metodo B: Usa GitHub (Definitivo)
1. Scarica le foto da OneDrive
2. Caricale su un repository GitHub pubblico
3. Usa i link GitHub raw:
   ```
   https://raw.githubusercontent.com/username/repo/main/photos/foto.jpg
   ```

### Metodo C: Azure Blob Storage (Professionale)
Se hai molte foto, considera:
1. Crea un Azure Storage Account
2. Carica le foto in un container pubblico
3. Usa i link blob:
   ```
   https://yourstore.blob.core.windows.net/photos/foto.jpg
   ```

---

## 🧪 Come Testare se il Link Funziona

### Test Rapido
Apri una nuova scheda del browser e incolla il link. Dovresti vedere **SOLO l'immagine**, non una pagina OneDrive.

✅ **Funziona:**
```
https://public.bn.files.1drv.com/y4m.../foto.jpg
→ Vedi solo l'immagine
```

❌ **Non funziona:**
```
https://1drv.ms/i/s!xxx
→ Vedi la pagina di visualizzazione OneDrive
```

### Test nell'App
1. Modifica `src/config/gallery.js`
2. Avvia l'app: `npm run dev`
3. Apri la console browser (F12)
4. Guarda se ci sono errori tipo "Failed to load image"

---

## 🔧 Risoluzione Problemi

### Errore: "Failed to load image"

**Causa:** Il link non è diretto all'immagine.

**Soluzione:**
1. Usa il metodo "Link Embed" (Soluzione 1)
2. Oppure aggiungi `?download=1` al link

### Errore: Immagine non si vede ma non ci sono errori

**Causa:** CORS (Cross-Origin Resource Sharing) bloccato.

**Soluzione:**
1. Verifica che il file sia condiviso pubblicamente
2. Prova con il metodo Embed invece del link di condivisione
3. Se persiste, considera di usare GitHub o Azure Blob

### Le foto funzionano ma sono lente

**Causa:** OneDrive non è ottimizzato per hosting di immagini.

**Soluzione:**
1. Riduci dimensione immagini (max 1MB)
2. Considera GitHub Pages o Azure CDN per prestazioni migliori

---

## 💡 Esempio Completo

Ecco un esempio pratico con link OneDrive reali:

```javascript
// src/config/gallery.js
export const GALLERY_CONFIG = {
  baseUrl: '', // Lascia vuoto se usi link OneDrive
  albums: [
    {
      id: 'vacanze-2024',
      name: 'Vacanze 2024',
      description: 'Estate al mare',
      // Link embed di OneDrive
      coverImage: 'https://public.bn.files.1drv.com/y4mXXXXX/cover.jpg',
      photos: [
        { 
          id: 1, 
          // Link embed
          filename: 'https://public.bn.files.1drv.com/y4mYYYYY/foto1.jpg',
          alt: 'Tramonto' 
        },
        { 
          id: 2, 
          // Link breve con download (viene convertito automaticamente)
          filename: 'https://1drv.ms/i/s!AjxKZZZZ',
          alt: 'Spiaggia' 
        },
        { 
          id: 3,
          // Puoi anche mixare con foto locali!
          filename: 'foto3.jpg', // Questa sarà presa da public/photos/vacanze-2024/
          alt: 'Gelato'
        }
      ]
    }
  ]
}
```

---

## 📋 Checklist Finale

Prima di deployare, verifica:

- [ ] Ogni link inizia con `https://public.bn.files.1drv.com` (embed) o ha `?download=1` (condivisione)
- [ ] Hai testato ogni link aprendolo in una nuova scheda del browser
- [ ] Le immagini si caricano correttamente nell'app in locale (`npm run dev`)
- [ ] Non ci sono errori nella console del browser (F12)
- [ ] I file sono condivisi pubblicamente su OneDrive

---

## 🆘 Serve Aiuto?

Se continui ad avere problemi:

1. **Apri la console del browser** (F12) e cerca errori
2. **Copia il link problematico** e testalo in una nuova scheda
3. **Verifica le impostazioni di condivisione** su OneDrive
4. Considera alternative come **GitHub** (più semplice) o **Azure Blob** (più performante)

---

## 🎯 TL;DR (Soluzione Veloce)

1. Su OneDrive: Click destro → **Incorpora** (Embed)
2. Copia l'URL dentro `src="..."` dal codice HTML
3. Incolla nella configurazione al posto di `filename`
4. Fatto! ✨
