#!/bin/bash

# Script di deploy automatico per Photo Gallery Functions
# Uso: ./deploy.sh <environment>
# Esempio: ./deploy.sh dev

set -e  # Esci in caso di errore

# Parametri
ENVIRONMENT=${1:-dev}
RESOURCE_GROUP="photo-gallery-${ENVIRONMENT}-rg"
LOCATION="westeurope"
STORAGE_ACCOUNT="photogallery${ENVIRONMENT}store"
FUNCTION_APP="photo-gallery-${ENVIRONMENT}-functions"
KEYVAULT_NAME="photogallery-${ENVIRONMENT}-kv"
CONTAINER_NAME="photos"

echo "🚀 Deploy Photo Gallery Functions - Environment: $ENVIRONMENT"
echo "=================================================="

# Step 1: Crea Resource Group
echo ""
echo "📦 Step 1/7: Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --output none

echo "✅ Resource Group created: $RESOURCE_GROUP"

# Step 2: Crea Storage Account
echo ""
echo "💾 Step 2/7: Creating Storage Account..."
az storage account create \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --output none

echo "✅ Storage Account created: $STORAGE_ACCOUNT"

# Step 3: Crea Container per le foto
echo ""
echo "📁 Step 3/7: Creating Blob Container..."
az storage container create \
  --name $CONTAINER_NAME \
  --account-name $STORAGE_ACCOUNT \
  --public-access blob \
  --output none

echo "✅ Blob Container created: $CONTAINER_NAME"

# Step 4: Crea Application Insights
echo ""
echo "📊 Step 4/7: Creating Application Insights..."
APPINSIGHTS_NAME="photo-gallery-${ENVIRONMENT}-ai"
az monitor app-insights component create \
  --app $APPINSIGHTS_NAME \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --output none

APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app $APPINSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query instrumentationKey \
  --output tsv)

echo "✅ Application Insights created: $APPINSIGHTS_NAME"

# Step 5: Crea Function App
echo ""
echo "⚡ Step 5/7: Creating Function App..."
az functionapp create \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --storage-account $STORAGE_ACCOUNT \
  --consumption-plan-location $LOCATION \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4 \
  --app-insights-key $APPINSIGHTS_KEY \
  --output none

echo "✅ Function App created: $FUNCTION_APP"

# Step 6: Configura Key Vault e Secrets
echo ""
echo "🔐 Step 6/7: Setting up Key Vault..."

# Crea Key Vault
az keyvault create \
  --name $KEYVAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --output none

# Abilita Managed Identity
az functionapp identity assign \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --output none

PRINCIPAL_ID=$(az functionapp identity show \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --query principalId \
  --output tsv)

# Dai permessi a Key Vault
az keyvault set-policy \
  --name $KEYVAULT_NAME \
  --object-id $PRINCIPAL_ID \
  --secret-permissions get list \
  --output none

# Ottieni connection string e salvala in Key Vault
CONNECTION_STRING=$(az storage account show-connection-string \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query connectionString \
  --output tsv)

az keyvault secret set \
  --vault-name $KEYVAULT_NAME \
  --name "AzureBlobStorage--ConnectionString" \
  --value "$CONNECTION_STRING" \
  --output none

SECRET_URI=$(az keyvault secret show \
  --vault-name $KEYVAULT_NAME \
  --name "AzureBlobStorage--ConnectionString" \
  --query id \
  --output tsv)

echo "✅ Key Vault configured: $KEYVAULT_NAME"

# Configura Application Settings
echo ""
echo "⚙️  Step 7/7: Configuring Function App Settings..."

BLOB_BASE_URL="https://${STORAGE_ACCOUNT}.blob.core.windows.net/${CONTAINER_NAME}"

az functionapp config appsettings set \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --settings \
  "AzureBlobStorage:ConnectionString=@Microsoft.KeyVault(SecretUri=$SECRET_URI)" \
  "AzureBlobStorage:ContainerName=$CONTAINER_NAME" \
  "AzureBlobStorage:BaseUrl=$BLOB_BASE_URL" \
  --output none

echo "✅ Function App settings configured"

# Deploy del codice
echo ""
echo "📦 Building and deploying code..."
dotnet publish -c Release --output ./publish

cd publish
func azure functionapp publish $FUNCTION_APP
cd ..

echo ""
echo "✅ Deploy completato con successo!"
echo ""
echo "📋 Riepilogo:"
echo "   Resource Group:    $RESOURCE_GROUP"
echo "   Storage Account:   $STORAGE_ACCOUNT"
echo "   Container:         $CONTAINER_NAME"
echo "   Function App:      $FUNCTION_APP"
echo "   Key Vault:         $KEYVAULT_NAME"
echo "   Blob Base URL:     $BLOB_BASE_URL"
echo ""
echo "🔗 Function URL:"
FUNCTION_URL=$(az functionapp function show \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --function-name GetGalleryStarter \
  --query invokeUrlTemplate \
  --output tsv 2>/dev/null || echo "Riavvia la Function App e riprova")

echo "   GET $FUNCTION_URL"
echo ""
echo "📸 Per caricare foto:"
echo "   az storage blob upload-batch \\"
echo "     --account-name $STORAGE_ACCOUNT \\"
echo "     --destination $CONTAINER_NAME/album-name \\"
echo "     --source ./local-photos/album-name"
echo ""
echo "🎉 Fatto!"
