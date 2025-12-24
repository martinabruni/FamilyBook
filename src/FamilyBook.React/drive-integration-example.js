/**
 * ESEMPIO DI INTEGRAZIONE CON GOOGLE DRIVE
 * 
 * Questo file mostra come integrare l'app con Google Drive API
 * per caricare dinamicamente le foto da una cartella condivisa.
 * 
 * PREREQUISITI:
 * 1. Creare un progetto su Google Cloud Console
 * 2. Abilitare Google Drive API
 * 3. Creare credenziali (API Key o OAuth 2.0)
 * 4. Condividere la cartella Drive in sola lettura
 */

// OPZIONE 1: Link Pubblici Diretti (Senza API)
// =============================================

/**
 * Converti un link di condivisione Google Drive in link diretto
 */
export const convertDriveLink = (shareUrl) => {
  // Da: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
  // A: https://drive.google.com/uc?export=view&id=FILE_ID
  
  const fileIdMatch = shareUrl.match(/\/d\/([^/]+)/)
  if (fileIdMatch) {
    return `https://drive.google.com/uc?export=view&id=${fileIdMatch[1]}`
  }
  return shareUrl
}

// Esempio di configurazione con link diretti
export const GALLERY_CONFIG_DRIVE = {
  albums: [
    {
      id: 'natale-2025',
      name: 'Natale 2025',
      coverImage: convertDriveLink('https://drive.google.com/file/d/YOUR_FILE_ID/view'),
      photos: [
        { 
          id: 1, 
          filename: convertDriveLink('https://drive.google.com/file/d/PHOTO_ID_1/view'),
          alt: 'Foto 1' 
        }
      ]
    }
  ]
}

// OPZIONE 2: Google Drive API (Backend Serverless)
// =================================================

/**
 * Azure Function per leggere da Google Drive
 * 
 * File: azure-functions/GetGallery/index.js
 */
const exampleAzureFunction = `
const { google } = require('googleapis');

module.exports = async function (context, req) {
  try {
    // Autenticazione con Service Account
    const auth = new google.auth.GoogleAuth({
      keyFile: 'path/to/service-account-key.json',
      scopes: ['https://www.googleapis.com/auth/drive.readonly'],
    });

    const drive = google.drive({ version: 'v3', auth });

    // ID della cartella principale su Google Drive
    const rootFolderId = process.env.GOOGLE_DRIVE_FOLDER_ID;

    // Lista sottocartelle (album)
    const albumsResponse = await drive.files.list({
      q: \`'\${rootFolderId}' in parents and mimeType='application/vnd.google-apps.folder'\`,
      fields: 'files(id, name, createdTime)',
      orderBy: 'createdTime desc'
    });

    // Per ogni album, carica le foto
    const albums = await Promise.all(
      albumsResponse.data.files.map(async (folder) => {
        const photosResponse = await drive.files.list({
          q: \`'\${folder.id}' in parents and (mimeType contains 'image/jpeg' or mimeType contains 'image/png')\`,
          fields: 'files(id, name, webContentLink, thumbnailLink, createdTime)',
          orderBy: 'createdTime'
        });

        // Trova la foto di copertina (prima foto o file chiamato 'cover')
        const coverPhoto = photosResponse.data.files.find(f => 
          f.name.toLowerCase().includes('cover')
        ) || photosResponse.data.files[0];

        return {
          id: folder.id,
          name: folder.name,
          description: \`Album con \${photosResponse.data.files.length} foto\`,
          coverImage: \`https://drive.google.com/uc?export=view&id=\${coverPhoto.id}\`,
          photos: photosResponse.data.files.map((photo, idx) => ({
            id: photo.id,
            filename: \`https://drive.google.com/uc?export=view&id=\${photo.id}\`,
            alt: photo.name,
            thumbnail: photo.thumbnailLink
          }))
        };
      })
    );

    context.res = {
      status: 200,
      body: { albums }
    };
  } catch (error) {
    context.log.error('Error fetching from Google Drive:', error);
    context.res = {
      status: 500,
      body: { error: 'Failed to fetch gallery data' }
    };
  }
};
`;

/**
 * Modifica al componente App.jsx per usare l'API
 */
const exampleAppModification = `
import { useState, useEffect } from 'react';

function App() {
  const [albums, setAlbums] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);

  useEffect(() => {
    const fetchGallery = async () => {
      try {
        const response = await fetch(
          process.env.VITE_API_URL || 'https://your-function.azurewebsites.net/api/GetGallery'
        );
        
        if (!response.ok) throw new Error('Failed to fetch gallery');
        
        const data = await response.json();
        setAlbums(data.albums);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchGallery();
  }, []);

  if (loading) return <div>Caricamento...</div>;
  if (error) return <div>Errore: {error}</div>;

  return (
    // ... resto del componente
  );
}
`;

// OPZIONE 3: OneDrive API (Microsoft Graph)
// ==========================================

/**
 * Azure Function per leggere da OneDrive
 */
const exampleOneDriveFunction = `
const { Client } = require('@microsoft/microsoft-graph-client');
require('isomorphic-fetch');

module.exports = async function (context, req) {
  try {
    // Autenticazione con App Registration
    const client = Client.init({
      authProvider: (done) => {
        done(null, process.env.GRAPH_ACCESS_TOKEN);
      }
    });

    // ID della cartella OneDrive
    const folderId = process.env.ONEDRIVE_FOLDER_ID;

    // Lista sottocartelle
    const folders = await client
      .api(\`/me/drive/items/\${folderId}/children\`)
      .filter("folder ne null")
      .get();

    const albums = await Promise.all(
      folders.value.map(async (folder) => {
        const photos = await client
          .api(\`/me/drive/items/\${folder.id}/children\`)
          .filter("file ne null and (endswith(name,'.jpg') or endswith(name,'.png'))")
          .get();

        return {
          id: folder.id,
          name: folder.name,
          coverImage: photos.value[0]?.['@microsoft.graph.downloadUrl'],
          photos: photos.value.map(photo => ({
            id: photo.id,
            filename: photo['@microsoft.graph.downloadUrl'],
            alt: photo.name
          }))
        };
      })
    );

    context.res = {
      status: 200,
      body: { albums }
    };
  } catch (error) {
    context.log.error('Error fetching from OneDrive:', error);
    context.res = {
      status: 500,
      body: { error: 'Failed to fetch gallery data' }
    };
  }
};
`;

// VARIABILI D'AMBIENTE
// ====================

/**
 * File .env.example
 */
const envExample = `
# Google Drive
GOOGLE_DRIVE_FOLDER_ID=your_folder_id_here
GOOGLE_SERVICE_ACCOUNT_KEY=path/to/key.json

# OneDrive
ONEDRIVE_FOLDER_ID=your_folder_id_here
GRAPH_ACCESS_TOKEN=your_access_token_here

# API Endpoint
VITE_API_URL=https://your-function.azurewebsites.net/api/GetGallery
`;

// ISTRUZIONI SETUP
// ================

export const SETUP_INSTRUCTIONS = {
  googleDrive: [
    '1. Vai su https://console.cloud.google.com',
    '2. Crea nuovo progetto o seleziona esistente',
    '3. Abilita Google Drive API',
    '4. Crea Service Account e scarica la chiave JSON',
    '5. Condividi la cartella Drive con l\'email del Service Account',
    '6. Copia l\'ID della cartella dall\'URL (dopo /folders/)',
    '7. Deploy Azure Function con le credenziali'
  ],
  oneDrive: [
    '1. Vai su https://portal.azure.com',
    '2. Registra nuova App in Azure AD',
    '3. Aggiungi permessi Microsoft Graph (Files.Read)',
    '4. Genera Client Secret',
    '5. Ottieni Access Token con OAuth 2.0',
    '6. Trova l\'ID della cartella OneDrive',
    '7. Deploy Azure Function con il token'
  ]
};

export {
  exampleAzureFunction,
  exampleOneDriveFunction,
  exampleAppModification,
  envExample
};
