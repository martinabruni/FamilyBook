# 🔐 Setup Secrets - Guida Completa

Questa guida spiega come gestire le configurazioni sensibili in sviluppo locale e in produzione.

## 🏠 Sviluppo Locale (User Secrets)

### Perché User Secrets?

- ✅ Non committate nel Git
- ✅ Specifici per macchina/utente
- ✅ Sicuri per sviluppo locale
- ✅ Integrati con .NET

### Setup Passo-Passo

#### 1. Inizializza User Secrets

```bash
cd PhotoGallery.Functions
dotnet user-secrets init
```

Questo crea un GUID nel file `.csproj` che identifica univocamente il progetto.

#### 2. Aggiungi la Connection String

```bash
# Sostituisci con la tua connection string reale
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=your-key-here;EndpointSuffix=core.windows.net"
```

#### 3. Verifica i Secrets

```bash
# Lista tutti i secrets
dotnet user-secrets list

# Output:
# AzureBlobStorage:ConnectionString = DefaultEndpointsProtocol=https;...
```

#### 4. Dove sono salvati?

**Windows:**
```
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
```

**macOS/Linux:**
```
~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
```

### Ottenere la Connection String di Azure

#### Opzione A: Azure Portal

1. Vai su [portal.azure.com](https://portal.azure.com)
2. Seleziona il tuo Storage Account
3. Nel menu a sinistra: **Access keys**
4. Copia la "Connection string" da Key1 o Key2

#### Opzione B: Azure CLI

```bash
# Ottieni la connection string
az storage account show-connection-string \
  --name yourstorageaccount \
  --resource-group your-resource-group \
  --query connectionString \
  --output tsv

# Output: DefaultEndpointsProtocol=https;AccountName=...

# Salvala direttamente in User Secrets
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "$(az storage account show-connection-string --name yourstorageaccount --resource-group your-resource-group --query connectionString -o tsv)"
```

### Sviluppo con Azurite (Storage Emulator)

Se vuoi testare localmente senza Azure:

```bash
# Installa Azurite
npm install -g azurite

# Avvia Azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log
```

**Connection String per Azurite:**
```bash
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "UseDevelopmentStorage=true"
```

---

## ☁️ Produzione (Azure Key Vault)

### Perché Azure Key Vault?

- ✅ Centralizzato e sicuro
- ✅ Audit trail completo
- ✅ Rotazione automatica delle chiavi
- ✅ Managed Identity (nessuna password)

### Setup Passo-Passo

#### 1. Crea Azure Key Vault

```bash
# Variabili
RESOURCE_GROUP="photo-gallery-rg"
KEYVAULT_NAME="photogallery-kv"
LOCATION="westeurope"
FUNCTION_APP_NAME="photo-gallery-functions"

# Crea Key Vault
az keyvault create \
  --name $KEYVAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION
```

#### 2. Abilita Managed Identity sulla Function App

```bash
# Abilita System-Assigned Managed Identity
az functionapp identity assign \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP

# Salva il Principal ID
PRINCIPAL_ID=$(az functionapp identity show \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query principalId \
  --output tsv)

echo "Principal ID: $PRINCIPAL_ID"
```

#### 3. Dai permessi alla Function App

```bash
# Permetti alla Function App di leggere i secrets
az keyvault set-policy \
  --name $KEYVAULT_NAME \
  --object-id $PRINCIPAL_ID \
  --secret-permissions get list
```

#### 4. Aggiungi i Secrets a Key Vault

```bash
# Ottieni la connection string dello Storage Account
CONNECTION_STRING=$(az storage account show-connection-string \
  --name yourstorageaccount \
  --resource-group $RESOURCE_GROUP \
  --query connectionString \
  --output tsv)

# Aggiungi al Key Vault (usa -- invece di : per i nomi)
az keyvault secret set \
  --vault-name $KEYVAULT_NAME \
  --name "AzureBlobStorage--ConnectionString" \
  --value "$CONNECTION_STRING"
```

**⚠️ IMPORTANTE:** Usa `--` invece di `:` nei nomi dei secrets di Key Vault!

#### 5. Configura la Function App

```bash
# Ottieni l'URI del secret
SECRET_URI=$(az keyvault secret show \
  --vault-name $KEYVAULT_NAME \
  --name "AzureBlobStorage--ConnectionString" \
  --query id \
  --output tsv)

# Configura Application Setting per usare Key Vault
az functionapp config appsettings set \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
  "AzureBlobStorage:ConnectionString=@Microsoft.KeyVault(SecretUri=$SECRET_URI)"
```

#### 6. Verifica

```bash
# Lista i settings (il valore non sarà visibile)
az functionapp config appsettings list \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "[?name=='AzureBlobStorage:ConnectionString'].{Name:name, Value:value}" \
  --output table

# Output:
# Name                                  Value
# ------------------------------------  --------------------------------------------------
# AzureBlobStorage:ConnectionString     @Microsoft.KeyVault(SecretUri=https://...)
```

---

## 🔄 Alternative: Application Settings Dirette

Se non vuoi usare Key Vault (meno sicuro):

```bash
# Ottieni connection string
CONNECTION_STRING=$(az storage account show-connection-string \
  --name yourstorageaccount \
  --resource-group your-resource-group \
  --query connectionString \
  --output tsv)

# Imposta direttamente nell'Application Settings
az functionapp config appsettings set \
  --name photo-gallery-functions \
  --resource-group photo-gallery-rg \
  --settings \
  "AzureBlobStorage:ConnectionString=$CONNECTION_STRING" \
  "AzureBlobStorage:ContainerName=photos" \
  "AzureBlobStorage:BaseUrl=https://yourstorageaccount.blob.core.windows.net/photos"
```

**⚠️ Attenzione:** La connection string sarà visibile nel portale Azure.

---

## 🧪 Testing della Configurazione

### Test Locale

```csharp
// Nel tuo codice, aggiungi logging
_logger.LogInformation("Connection String configured: {IsConfigured}", 
    !string.IsNullOrEmpty(_configuration["AzureBlobStorage:ConnectionString"]));
```

```bash
# Esegui la function
dotnet run

# Dovresti vedere nei log:
# Connection String configured: True
```

### Test in Azure

```bash
# Controlla i log della Function App
az functionapp log tail \
  --name photo-gallery-functions \
  --resource-group photo-gallery-rg

# Oppure vai su Portal → Function App → Log Stream
```

---

## 📋 Checklist Setup

### Sviluppo Locale
- [ ] `dotnet user-secrets init` eseguito
- [ ] Connection string aggiunta a User Secrets
- [ ] `dotnet user-secrets list` mostra la configurazione
- [ ] La function si avvia senza errori

### Produzione (Key Vault)
- [ ] Key Vault creato
- [ ] Managed Identity abilitata sulla Function App
- [ ] Permessi impostati (get, list)
- [ ] Secret aggiunto a Key Vault con nome corretto (`--` invece di `:`)
- [ ] Application Setting configurato con `@Microsoft.KeyVault(...)`
- [ ] Function App riavviata
- [ ] Log verificati per confermare lettura secret

### Produzione (Application Settings)
- [ ] Connection string ottenuta da Azure Storage
- [ ] Application Settings configurate
- [ ] Function App riavviata
- [ ] Test endpoint eseguito

---

## 🔍 Troubleshooting

### Errore: "ConnectionString is not configured"

**Causa:** User Secrets non configurati o nome chiave errato.

**Soluzione:**
```bash
# Verifica i secrets
dotnet user-secrets list

# Deve mostrare:
# AzureBlobStorage:ConnectionString = ...

# Se vuoto, aggiungi:
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "YOUR_CONNECTION_STRING"
```

### Errore: "KeyVault access denied"

**Causa:** Managed Identity non ha permessi sul Key Vault.

**Soluzione:**
```bash
# Verifica l'identità
az functionapp identity show \
  --name photo-gallery-functions \
  --resource-group photo-gallery-rg

# Ri-imposta i permessi
az keyvault set-policy \
  --name photogallery-kv \
  --object-id <PRINCIPAL_ID> \
  --secret-permissions get list
```

### Errore: "Secret not found in KeyVault"

**Causa:** Nome del secret errato (usare `--` invece di `:`)

**Soluzione:**
```bash
# Lista i secrets
az keyvault secret list \
  --vault-name photogallery-kv \
  --query "[].name" \
  --output table

# Il nome deve essere: AzureBlobStorage--ConnectionString
# NON: AzureBlobStorage:ConnectionString
```

---

## 🎯 Best Practices

1. **Mai committare secrets** in Git
2. **Usa User Secrets** in sviluppo locale
3. **Usa Key Vault** in produzione
4. **Ruota le chiavi** regolarmente (ogni 90 giorni)
5. **Monitora l'accesso** ai secrets con Azure Monitor
6. **Usa Managed Identity** invece di connection string quando possibile
7. **Documenta quali secrets** sono necessari nel README

---

## 📚 Riferimenti

- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview)
- [Managed Identity](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview)
- [Key Vault References in App Service](https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references)
