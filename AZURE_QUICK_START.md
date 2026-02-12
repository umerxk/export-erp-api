# Azure Deployment - Quick Start

## What's Been Updated

✅ Added SQL Server support (for Azure SQL Database)
✅ Updated `Program.cs` to use Azure connection string when available
✅ Added `ConnectionStrings` section to `appsettings.json`
✅ Added `Microsoft.EntityFrameworkCore.SqlServer` package

## Quick Deployment Steps

### 1. Restore Packages Locally
```bash
cd ExportERP.Api
dotnet restore
```

### 2. Create Azure Resources (via Portal)

**A. Create SQL Database:**
- Resource: SQL Database
- Tier: Serverless (Free)
- Server: Create new (remember username/password!)

**B. Create App Service:**
- Resource: Web App
- Runtime: .NET 10 (LTS)
- Plan: F1 Free (Linux)

**C. Configure Connection String:**
- App Service → Configuration → Application settings
- Add: `ConnectionStrings__DefaultConnection` = [your SQL connection string]

**D. Deploy from GitHub:**
- App Service → Deployment Center
- Connect your `export-erp-api` repository
- Branch: `main`

### 3. Run Migrations

After deployment, connect via SSH and run:
```bash
cd /home/site/wwwroot
dotnet ef database update
```

## Your API URL

After deployment, your API will be available at:
```
https://export-erp-api.azurewebsites.net
```

Update your frontend `api.js` to use this URL!

## Important Notes

- **Free tier limitations**: App may pause after inactivity
- **Database**: Auto-pauses after 1 hour (takes a few seconds to resume)
- **Swagger**: Available at `/swagger` endpoint

For detailed instructions, see `DEPLOYMENT.md`
