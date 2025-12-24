# 🏗️ Architettura Photo Gallery Functions

## Diagramma Completo

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            CLIENT (React App)                                │
└───────────────────────────────┬─────────────────────────────────────────────┘
                                │
                                │ HTTP GET /api/gallery
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        AZURE DURABLE FUNCTIONS                               │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────────┐    │
│  │  HTTP Trigger Layer                                                 │    │
│  │                                                                      │    │
│  │  ┌──────────────────────────────────────────────────────────┐      │    │
│  │  │  GetGalleryStarter.cs                                     │      │    │
│  │  │  - Riceve HTTP GET request                                │      │    │
│  │  │  - Avvia l'orchestrazione                                 │      │    │
│  │  │  - Ritorna status URLs                                    │      │    │
│  │  └──────────────────────────────────────────────────────────┘      │    │
│  └──────────────────────┬───────────────────────────────────────────────┘    │
│                         │                                                    │
│                         │ Starts orchestration                               │
│                         ▼                                                    │
│  ┌────────────────────────────────────────────────────────────────────┐    │
│  │  Orchestrator Layer                                                 │    │
│  │                                                                      │    │
│  │  ┌──────────────────────────────────────────────────────────┐      │    │
│  │  │  GetGalleryOrchestrator.cs                                │      │    │
│  │  │  - Coordina le activity in modo sequenziale/parallelo     │      │    │
│  │  │  - Gestisce il flusso di lavoro                           │      │    │
│  │  │  - Aggrega i risultati                                    │      │    │
│  │  └──────────────────────────────────────────────────────────┘      │    │
│  └──────────┬────────────────────┬────────────────────┬────────────────┘    │
│             │                    │                    │                      │
│             ▼                    ▼                    ▼                      │
│  ┌─────────────────┐  ┌──────────────────┐  ┌──────────────────┐          │
│  │ GetAlbumsActivity│  │GetAlbumDetails   │  │BuildGalleryConfig│          │
│  │                  │  │Activity (x N)    │  │Activity          │          │
│  │ Step 1:          │  │                  │  │                  │          │
│  │ Get album names  │  │ Step 2:          │  │ Step 3:          │          │
│  │                  │  │ Get photos       │  │ Build final JSON │          │
│  │ ┌──────────────┐ │  │ (Parallel)       │  │                  │          │
│  │ │IPhotoService │ │  │ ┌──────────────┐ │  │                  │          │
│  │ └──────────────┘ │  │ │IPhotoService │ │  │                  │          │
│  └─────────────────┘  └─┴──────────────┴─┘  └──────────────────┘          │
│             │                    │                    │                      │
└─────────────┼────────────────────┼────────────────────┼──────────────────────┘
              │                    │                    │
              ▼                    ▼                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                          APPLICATION LAYER                                   │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────────┐    │
│  │  PhotoService.cs                                                    │    │
│  │  - Implements IPhotoService                                         │    │
│  │  - Business logic per album e foto                                  │    │
│  │  - Formattazione nomi album                                         │    │
│  │                                                                      │    │
│  │  Methods:                                                            │    │
│  │  - GetAlbumNamesAsync()                                             │    │
│  │  - GetAlbumAsync(albumName)                                         │    │
│  │  - GetGalleryConfigAsync()                                          │    │
│  │                                                                      │    │
│  │  ┌──────────────────────────┐                                       │    │
│  │  │   IPhotoRepository       │                                       │    │
│  │  └──────────────────────────┘                                       │    │
│  └──────────────────┬───────────────────────────────────────────────────┘    │
└─────────────────────┼──────────────────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        INFRASTRUCTURE LAYER                                  │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────────┐    │
│  │  PhotoRepository.cs                                                 │    │
│  │  - Implements IPhotoRepository                                      │    │
│  │  - Accesso ad Azure Blob Storage                                    │    │
│  │  - BlobServiceClient, BlobContainerClient                           │    │
│  │                                                                      │    │
│  │  Methods:                                                            │    │
│  │  - GetAlbumNamesAsync()        → Lista cartelle blob               │    │
│  │  - GetPhotosFromAlbumAsync()   → Lista blob in una cartella        │    │
│  │                                                                      │    │
│  │  Filters:                                                            │    │
│  │  - IsImageFile()               → .jpg, .png, .gif, etc.            │    │
│  │  - IsCoverImage()              → Skip cover.jpg                     │    │
│  └──────────────────┬───────────────────────────────────────────────────┘    │
└─────────────────────┼──────────────────────────────────────────────────────┘
                      │
                      │ Azure Blob Storage SDK
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        AZURE BLOB STORAGE                                    │
│                                                                              │
│  Container: photos/                                                          │
│  ├── natale-2025/                                                           │
│  │   ├── cover.jpg        ← Cover dell'album                               │
│  │   ├── foto1.jpg                                                          │
│  │   ├── foto2.jpg                                                          │
│  │   └── foto3.jpg                                                          │
│  │                                                                           │
│  ├── estate-2024/                                                           │
│  │   ├── cover.png                                                          │
│  │   ├── foto1.jpg                                                          │
│  │   └── foto2.jpg                                                          │
│  │                                                                           │
│  └── compleanno-2024/                                                       │
│      └── ...                                                                 │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Flusso di Esecuzione Dettagliato

```
1. CLIENT → HTTP GET /api/gallery
   │
   ├─→ GetGalleryStarter (HTTP Trigger)
   │   └─→ Avvia orchestrazione
   │       └─→ Ritorna: { 
   │           "id": "abc123",
   │           "statusQueryGetUri": "http://.../instances/abc123"
   │          }
   │
2. CLIENT → Polling su statusQueryGetUri
   │
   ├─→ GetGalleryOrchestrator
   │   │
   │   ├─→ STEP 1: GetAlbumsActivity
   │   │   └─→ IPhotoService.GetAlbumNamesAsync()
   │   │       └─→ IPhotoRepository.GetAlbumNamesAsync()
   │   │           └─→ Azure Blob: GetBlobsByHierarchyAsync()
   │   │               └─→ Ritorna: ["natale-2025", "estate-2024", ...]
   │   │
   │   ├─→ STEP 2: GetAlbumDetailsActivity (Parallelo per ogni album)
   │   │   │
   │   │   ├─→ Per "natale-2025":
   │   │   │   └─→ IPhotoService.GetAlbumAsync("natale-2025")
   │   │   │       └─→ IPhotoRepository.GetPhotosFromAlbumAsync("natale-2025")
   │   │   │           └─→ Azure Blob: GetBlobsAsync(prefix: "natale-2025/")
   │   │   │               └─→ Ritorna: [Photo1, Photo2, Photo3]
   │   │   │
   │   │   ├─→ Per "estate-2024": (stesso processo in parallelo)
   │   │   │
   │   │   └─→ Per "compleanno-2024": (stesso processo in parallelo)
   │   │
   │   └─→ STEP 3: BuildGalleryConfigActivity
   │       └─→ Aggrega tutti gli album in GalleryConfig
   │           └─→ Ritorna: {
   │               "baseUrl": "https://...",
   │               "albums": [Album1, Album2, Album3]
   │              }
   │
3. CLIENT → Riceve GalleryConfig completo
   └─→ Usa i dati per popolare la UI
```

## Dependency Injection Flow

```
Program.cs (Startup)
│
├─→ services.AddSingleton<IPhotoRepository, PhotoRepository>()
│   └─→ PhotoRepository(IConfiguration)
│       └─→ Legge ConnectionString da config/secrets
│           └─→ Crea BlobServiceClient
│
├─→ services.AddSingleton<IPhotoService, PhotoService>()
│   └─→ PhotoService(IPhotoRepository, IConfiguration)
│       └─→ Riceve IPhotoRepository iniettato
│
└─→ Activities registrate automaticamente
    │
    ├─→ GetAlbumsActivity(IPhotoService, ILogger)
    │   └─→ Riceve IPhotoService iniettato
    │
    ├─→ GetAlbumDetailsActivity(IPhotoService, ILogger)
    │   └─→ Riceve IPhotoService iniettato
    │
    └─→ BuildGalleryConfigActivity(IConfiguration, ILogger)
        └─→ Riceve IConfiguration iniettato
```

## Configurazione e Secrets

```
Development (locale)
│
├─→ appsettings.json
│   └─→ AzureBlobStorage:ConnectionString: "insecrets"
│
├─→ User Secrets
│   └─→ AzureBlobStorage:ConnectionString: "DefaultEndpoints..."
│       └─→ Letto da: ~/.microsoft/usersecrets/<guid>/secrets.json
│
└─→ PhotoRepository
    └─→ Riceve ConnectionString da IConfiguration


Production (Azure)
│
├─→ Azure Key Vault
│   └─→ Secret: "AzureBlobStorage--ConnectionString"
│
├─→ Function App Settings
│   └─→ AzureBlobStorage:ConnectionString: "@Microsoft.KeyVault(...)"
│
├─→ Managed Identity
│   └─→ Function App → Accede a Key Vault
│       └─→ Nessuna password/secret nel codice
│
└─→ PhotoRepository
    └─→ Riceve ConnectionString (transparente)
```

## Modelli di Dominio

```
GalleryConfig
├── BaseUrl: string
└── Albums: List<Album>
    │
    └─→ Album
        ├── Id: string
        ├── Name: string
        ├── Description: string
        ├── CoverImageUrl: string
        ├── PhotoCount: int
        └── Photos: List<Photo>
            │
            └─→ Photo
                ├── Id: string
                ├── FileName: string
                ├── Url: string
                ├── Alt: string
                ├── CreatedAt: DateTime
                └── SizeBytes: long
```

## Vantaggi dell'Architettura

### ✅ Separation of Concerns
- **Domain**: Modelli e interfacce (business logic)
- **Infrastructure**: Implementazioni concrete (Azure Blob)
- **Application**: Servizi applicativi
- **Functions**: Presentation layer (HTTP/Orchestration)

### ✅ Dependency Inversion
- Activities dipendono da IPhotoService (interfaccia)
- PhotoService dipende da IPhotoRepository (interfaccia)
- Facile testing con mock/stub

### ✅ Scalabilità
- Orchestrator gestisce attività parallele
- Ogni album processato in parallelo (Step 2)
- Auto-scaling di Azure Functions

### ✅ Resilienza
- Durable Functions gestisce retry automatici
- State management persistente
- Idempotenza delle activities

### ✅ Sicurezza
- Secrets in Key Vault
- Managed Identity (no passwords)
- HTTPS obbligatorio
- Function-level authorization
