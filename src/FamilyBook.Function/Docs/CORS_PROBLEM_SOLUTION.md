# 🚨 TROUBLESHOOTING: "Ho copiato il link ma le foto non si vedono"

## Il Problema

Hai fatto tutto bene:
- ✅ Link OneDrive ottenuto con "Incorpora"
- ✅ Link si apre nel browser e vedi la foto
- ✅ Nessun errore nella console
- ❌ Nell'app le foto non si vedono

**Causa: CORS (Cross-Origin Resource Sharing)**

OneDrive blocca le richieste da origini diverse (come `localhost:5173`), anche se il link è "pubblico".

```
┌─────────────┐
│   Browser   │
│ localhost   │ ──── GET foto.jpg ───> ❌ CORS BLOCCATO
└─────────────┘                        │
                                       │
                                  ┌────▼─────┐
                                  │ OneDrive │
                                  └──────────┘
```

---

## Come Capire se è CORS

### Test 1: Apri Console Browser
1. Premi F12
2. Vai alla tab "Console"
3. Vedi errori tipo:

```
Access to image at 'https://onedrive.live.com/...' from origin 'http://localhost:5173' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present.
```

### Test 2: Ispeziona Elemento
1. Click destro sull'immagine mancante
2. "Ispeziona elemento"
3. Vedi qualcosa tipo:

```html
<img src="https://public.bn.files.1drv.com/..." alt="foto" style="display: none;">
```

L'immagine c'è ma non viene mostrata = CORS

---

## ✅ SOLUZIONI (In ordine di facilità)

### 1. GitHub (5 minuti) ⭐ CONSIGLIATA

**Pro:**
- Gratis (1GB)
- Zero CORS
- Setup immediato

**Passi:**
1. Crea repository pubblico su GitHub
2. Carica foto in `photos/album/`
3. Usa URL:
   ```
   https://raw.githubusercontent.com/USERNAME/REPO/main/photos/album/foto.jpg
   ```

📖 [**Guida Completa GitHub**](GITHUB_SETUP.md)

---

### 2. Imgur (2 minuti)

**Pro:**
- Velocissimo
- Nessuna registrazione richiesta

**Passi:**
1. Vai su [imgur.com](https://imgur.com)
2. Upload foto
3. Click destro → "Copia indirizzo immagine"
4. Usa URL tipo:
   ```
   https://i.imgur.com/ABC123.jpg
   ```

**Contro:**
- Compressione immagini
- Non ideale per archivio permanente

---

### 3. Azure Blob Storage (15 minuti)

**Pro:**
- Professionale
- Scalabile
- Costa pochissimo

**Passi:**
1. Crea Azure Storage Account
2. Crea container pubblico
3. Carica foto
4. Configura CORS
5. Usa URL:
   ```
   https://storage.blob.core.windows.net/photos/album/foto.jpg
   ```

📖 [**Guida Completa Azure**](AZURE_BLOB_SETUP.md)

---

### 4. Cloudflare R2 (Alternativa ad Azure)

**Pro:**
- Simile ad Azure Blob
- Nessun costo di bandwidth

**Passi:**
1. Crea account Cloudflare
2. Crea bucket R2
3. Carica foto
4. Usa URL pubblico

---

### 5. GitHub Pages

**Pro:**
- CDN gratis
- Veloce

**Passi:**
1. Crea repository con foto
2. Abilita GitHub Pages
3. Usa URL:
   ```
   https://username.github.io/repo/photos/album/foto.jpg
   ```

---

## ❌ Cosa NON Funziona

| Servizio | Link Diretto | CORS | Usabile |
|----------|--------------|------|---------|
| OneDrive | ✅ Sì | ❌ Bloccato | ❌ No |
| Google Drive | ✅ Sì | ❌ Bloccato | ❌ No |
| Dropbox | ✅ Sì | ❌ Bloccato | ❌ No |
| iCloud | ✅ Sì | ❌ Bloccato | ❌ No |
| GitHub | ✅ Sì | ✅ Permesso | ✅ Sì |
| Imgur | ✅ Sì | ✅ Permesso | ✅ Sì |
| Azure Blob | ✅ Sì | ✅ Configurabile | ✅ Sì |

---

## 🔍 Come Verificare CORS

Usa questo comando curl per testare:

```bash
curl -I -H "Origin: http://localhost:5173" \
  "https://public.bn.files.1drv.com/y4m.../foto.jpg"
```

**Cerca questa header:**
```
Access-Control-Allow-Origin: *
```

Se NON c'è = CORS bloccato ❌

---

## 💡 Workaround (Non Consigliati)

### Opzione A: Proxy Server

Crea un server Node.js che scarica le immagini e le serve:

```javascript
// server.js
const express = require('express');
const fetch = require('node-fetch');
const app = express();

app.get('/proxy', async (req, res) => {
  const imageUrl = req.query.url;
  const response = await fetch(imageUrl);
  const buffer = await response.buffer();
  res.set('Content-Type', 'image/jpeg');
  res.send(buffer);
});

app.listen(3000);
```

**Contro:**
- Devi mantenere un server
- Costi aggiuntivi
- Più lento

### Opzione B: Browser Extension

Installa estensione per disabilitare CORS (solo sviluppo):

**⚠️ NON FUNZIONA per gli utenti finali!**

---

## 📊 Comparazione Soluzioni

| Soluzione | Setup | Costo | CORS | Performance | Limite |
|-----------|-------|-------|------|-------------|--------|
| GitHub | 5min | 🆓 | ✅ | ⭐⭐⭐⭐ | 1GB |
| Imgur | 2min | 🆓 | ✅ | ⭐⭐⭐ | Illimitato* |
| Azure Blob | 15min | €0.01/GB | ✅ | ⭐⭐⭐⭐⭐ | Illimitato |
| OneDrive | N/A | N/A | ❌ | N/A | **NON USABILE** |

*Imgur comprime le immagini

---

## 🎯 Raccomandazione Finale

**Per la maggior parte degli utenti:**
1. **Usa GitHub** se hai meno di 1GB di foto
2. **Usa Azure Blob** se hai più foto o vuoi qualcosa di professionale
3. **Usa Imgur** solo per test rapidi

**NON perdere tempo** cercando di far funzionare OneDrive/Google Drive - non è possibile bypassare CORS senza server intermedio.

---

## 🆘 Ancora Problemi?

### Checklist Finale

- [ ] Ho verificato che il link si apre nel browser?
- [ ] Ho controllato la console browser (F12)?
- [ ] Sto usando GitHub/Azure/Imgur invece di OneDrive?
- [ ] Ho configurato correttamente `baseUrl` in `gallery.js`?
- [ ] L'app è in esecuzione con `npm run dev`?
- [ ] Il repository GitHub è PUBBLICO (se uso GitHub)?
- [ ] Il container Azure è pubblico (se uso Azure)?

### Se nulla funziona

1. **Riparti da zero** con la soluzione GitHub
2. Segui **passo-passo** la guida in [GITHUB_SETUP.md](GITHUB_SETUP.md)
3. Testa con **una singola foto** prima di caricare tutto

---

## 📞 Debug Info Utili

Quando chiedi aiuto, fornisci:

```
1. Servizio usato: GitHub / Azure / OneDrive / Altro
2. URL di esempio foto: [il tuo URL]
3. Errore console: [copia/incolla errore]
4. Browser: Chrome / Firefox / Safari
5. Risposta curl:
   curl -I -H "Origin: http://localhost:5173" "[TUO_URL]"
```

---

## ✨ La Soluzione Più Veloce

```bash
# 1. Crea repo GitHub
# 2. Carica foto
# 3. In gallery.js:

export const GALLERY_CONFIG = {
  baseUrl: 'https://raw.githubusercontent.com/TUO_USERNAME/photos/main',
  albums: [/* ... */]
}

# 4. npm run dev
# 5. ✅ Funziona!
```

**Fine. Non serve altro.** 🎉
