# Azure Deployment Guide for Export ERP API

This guide will help you deploy your .NET API to Azure App Service using the free tier.

## Prerequisites

1. Azure account with free subscription
2. Azure CLI installed (or use Azure Portal)
3. .NET SDK 10.0 installed locally
4. Git repository pushed to GitHub (already done)

## Option 1: Deploy via Azure Portal (Easiest)

### Step 1: Create Azure SQL Database (Free Tier)

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"**
3. Search for **"SQL Database"** and select it
4. Click **"Create"**
5. Fill in the details:
   - **Subscription**: Your free subscription
   - **Resource Group**: Create new (e.g., `export-erp-rg`)
   - **Database name**: `export-erp-db`
   - **Server**: Create new server
     - Server name: `export-erp-server` (must be globally unique)
     - Location: Choose closest to you
     - Authentication: SQL authentication
     - Admin username: `sqladmin` (or your choice)
     - Password: Create a strong password (save this!)
   - **Want to use SQL elastic pool?**: No
   - **Compute + storage**: Click "Configure database"
     - Select **"Serverless"** tier (Free tier)
     - Max vCores: 1
     - Auto-pause delay: 1 hour
     - Click "Apply"
   - **Backup storage redundancy**: Locally-redundant backup storage
6. Click **"Review + create"**, then **"Create"**

### Step 2: Get Connection String

1. After database is created, go to your SQL Database resource
2. Click on **"Connection strings"** in the left menu
3. Copy the **ADO.NET** connection string
4. Replace `{your_password}` with your admin password
5. Save this connection string - you'll need it later

### Step 3: Create Azure App Service

1. In Azure Portal, click **"Create a resource"**
2. Search for **"Web App"** and select it
3. Click **"Create"**
4. Fill in the details:
   - **Subscription**: Your free subscription
   - **Resource Group**: Same as database (e.g., `export-erp-rg`)
   - **Name**: `export-erp-api` (must be globally unique)
   - **Publish**: Code
   - **Runtime stack**: `.NET 10 (LTS)`
   - **Operating System**: Linux (free tier)
   - **Region**: Same as your database
   - **App Service Plan**: 
     - Click "Create new"
     - Name: `export-erp-plan`
     - **SKU and size**: Click "Change size"
       - Select **"Dev/Test"** tab
       - Choose **"F1 Free"** tier
       - Click "Apply"
     - Click "OK"
5. Click **"Review + create"**, then **"Create"**

### Step 4: Configure Connection String in App Service

1. After App Service is created, go to your App Service resource
2. In the left menu, go to **"Configuration"** → **"Application settings"**
3. Click **"+ New application setting"**
4. Add:
   - **Name**: `ConnectionStrings__DefaultConnection`
   - **Value**: Paste your SQL Database connection string
5. Click **"OK"**, then **"Save"**

### Step 5: Deploy from GitHub

1. In your App Service, go to **"Deployment Center"** in the left menu
2. Select **"GitHub"** as source
3. Authorize Azure to access your GitHub account
4. Select:
   - **Organization**: Your GitHub username
   - **Repository**: `export-erp-api`
   - **Branch**: `main`
   - **Build provider**: GitHub Actions (recommended)
5. Click **"Save"**
6. Azure will automatically create a GitHub Actions workflow and deploy your app

### Step 6: Update Database Migrations

After deployment, you need to run migrations. You can do this via SSH:

1. In App Service, go to **"SSH"** in the left menu
2. Connect to the container
3. Navigate to your app directory: `cd /home/site/wwwroot`
4. Run migrations: `dotnet ef database update`

**OR** use Azure Cloud Shell:

```bash
az webapp ssh --name export-erp-api --resource-group export-erp-rg
cd /home/site/wwwroot
dotnet ef database update
```

## Option 2: Deploy via Azure CLI

### Step 1: Login to Azure

```bash
az login
```

### Step 2: Create Resource Group

```bash
az group create --name export-erp-rg --location eastus
```

### Step 3: Create SQL Server and Database

```bash
# Create SQL Server
az sql server create \
  --name export-erp-server \
  --resource-group export-erp-rg \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YOUR_STRONG_PASSWORD

# Create Firewall Rule (allow Azure services)
az sql server firewall-rule create \
  --resource-group export-erp-rg \
  --server export-erp-server \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Create Database (Serverless Free Tier)
az sql db create \
  --resource-group export-erp-rg \
  --server export-erp-server \
  --name export-erp-db \
  --service-objective Free \
  --capacity 1 \
  --family Gen5
```

### Step 4: Create App Service Plan and Web App

```bash
# Create App Service Plan (Free Tier)
az appservice plan create \
  --name export-erp-plan \
  --resource-group export-erp-rg \
  --sku FREE \
  --is-linux

# Create Web App
az webapp create \
  --name export-erp-api \
  --resource-group export-erp-rg \
  --plan export-erp-plan \
  --runtime "DOTNET|10.0"
```

### Step 5: Get Connection String and Configure

```bash
# Get connection string
CONNECTION_STRING=$(az sql db show-connection-string \
  --server export-erp-server \
  --name export-erp-db \
  --client ado-net \
  --output tsv)

# Set connection string in App Service
az webapp config connection-string set \
  --name export-erp-api \
  --resource-group export-erp-rg \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="$CONNECTION_STRING"
```

### Step 6: Deploy from GitHub

```bash
az webapp deployment source config \
  --name export-erp-api \
  --resource-group export-erp-rg \
  --repo-url https://github.com/YOUR_USERNAME/export-erp-api \
  --branch main \
  --manual-integration
```

## Important Notes

### Database Considerations

- **SQLite**: Not recommended for Azure App Service as files are ephemeral
- **Azure SQL Database Free Tier**: 
  - Serverless tier with auto-pause after 1 hour of inactivity
  - First 32GB storage is free
  - Perfect for development/testing

### Free Tier Limitations

- **App Service F1 Free**:
  - 1 GB storage
  - 60 minutes compute time per day
  - Apps may be paused after inactivity
  - Custom domains not supported

- **SQL Database Free**:
  - Auto-pauses after 1 hour inactivity
  - May take a few seconds to resume
  - 32GB storage included

### CORS Configuration

Your app already has CORS configured. After deployment, update the frontend API URL to point to your Azure App Service URL (e.g., `https://export-erp-api.azurewebsites.net`).

### Environment Variables

The app will automatically use the connection string from Azure App Service configuration when deployed. For local development, it will fall back to SQLite.

## Troubleshooting

1. **App won't start**: Check logs in App Service → Log stream
2. **Database connection fails**: Verify connection string in Configuration
3. **Migrations not applied**: Run migrations via SSH or add migration script to deployment
4. **CORS errors**: Ensure frontend URL is allowed in CORS policy

## Next Steps

After deployment:
1. Update your frontend `api.js` to use the Azure API URL
2. Test the API endpoints
3. Consider setting up CI/CD for automatic deployments
