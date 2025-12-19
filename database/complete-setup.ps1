# Complete Database Setup and Scaffolding Script
# Handles everything: database creation, schema, tables, triggers, seeding, and scaffolding

param(
    [string]$DbHost = "localhost",
    [string]$DbName = "billing_system",
    [string]$DbUser = "postgres",
    [string]$DbPass = "postgres",
    [string]$BackendPath = "..\backend\PartitionTableFullStack.API"
)

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host " COMPLETE DATABASE SETUP & SCAFFOLDING" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

$env:PGPASSWORD = $DbPass

# Step 0: Terminate existing connections and drop database
Write-Host "Step 0: Dropping and recreating database..." -ForegroundColor Yellow
$terminateQuery = @"
SELECT pg_terminate_backend(pid) FROM pg_stat_activity 
WHERE datname = '$DbName' AND pid <> pg_backend_pid();
"@
psql -U $DbUser -h $DbHost -d postgres -c $terminateQuery 2>$null
psql -U $DbUser -h $DbHost -d postgres -c "DROP DATABASE IF EXISTS $DbName;" 2>$null
psql -U $DbUser -h $DbHost -d postgres -c "CREATE DATABASE $DbName;" 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Database creation had warnings" -ForegroundColor Yellow
}

Write-Host "✅ Database created" -ForegroundColor Green

# Step 1: Run database setup
Write-Host ""
Write-Host "Step 1: Setting up tables, triggers, seed data..." -ForegroundColor Yellow
psql -U $DbUser -h $DbHost -d $DbName -f setup-database.sql

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Database setup failed!" -ForegroundColor Red
    $env:PGPASSWORD = $null
    exit 1
}

Write-Host "✅ Database setup complete!" -ForegroundColor Green

$env:PGPASSWORD = $null

# Step 2: Clean old generated files
Write-Host ""
Write-Host "Step 2: Cleaning old generated files..." -ForegroundColor Yellow
Push-Location $BackendPath
Remove-Item Models\*.cs -Force -ErrorAction SilentlyContinue
Remove-Item Data\Application*.cs -Force -ErrorAction SilentlyContinue
Pop-Location
Write-Host "✅ Cleaned" -ForegroundColor Green

# Step 3: Run the scaffold-models.ps1 script (handles both regular and partitioned tables)
Write-Host ""
Write-Host "Step 3: Running scaffolding (regular + partitioned tables)..." -ForegroundColor Yellow
Write-Host ""

# Call scaffold-models.ps1 with same parameters
$scaffoldScript = Join-Path $PSScriptRoot "scaffold-models.ps1"
& $scaffoldScript -DbHost $DbHost -DbName $DbName -DbUser $DbUser -DbPass $DbPass -OutputDir $BackendPath -Force

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Scaffolding failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Green
Write-Host " ✅ ALL COMPLETE!" -ForegroundColor Green
Write-Host "========================================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Summary:"
Write-Host "  ✓ Database dropped and recreated" -ForegroundColor Green
Write-Host "  ✓ All tables created" -ForegroundColor Green
Write-Host "  ✓ Triggers and functions installed" -ForegroundColor Green
Write-Host "  ✓ Seed data inserted" -ForegroundColor Green
Write-Host "  ✓ Models scaffolded (regular + partitioned)" -ForegroundColor Green
Write-Host "  ✓ ApplicationDbContext.Billing.cs generated with PKs/FKs" -ForegroundColor Green
Write-Host ""
