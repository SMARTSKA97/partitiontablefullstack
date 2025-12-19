# =============================================================================
# EF CORE SCAFFOLDING - With Partitioned Table Support
# Scaffolds regular tables via EF Core, then generates billing models from schema
# =============================================================================

param(
    [string]$DbHost = "localhost",
    [string]$DbName = "billing_system",
    [string]$DbUser = "postgres",
    [string]$DbPass = "postgres",
    [string]$OutputDir = "..\backend\PartitionTableFullStack.API",
    [string]$ContextName = "ApplicationDbContext",
    [switch]$Force
)

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host " EF CORE SCAFFOLDING (With Partitioned Table Support)" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

$env:PGPASSWORD = $DbPass

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "   Database: $DbName @ $DbHost"
Write-Host "   Output: $OutputDir"
Write-Host ""

# Step 1: Discover NON-partitioned tables (regular tables only)
Write-Host "🔍 Step 1: Discovering regular tables..." -ForegroundColor Yellow

$regularQuery = @"
SELECT n.nspname, c.relname
FROM pg_class c
JOIN pg_namespace n ON n.oid = c.relnamespace
WHERE c.relkind = 'r'
  AND NOT c.relispartition
  AND n.nspname NOT IN ('pg_catalog', 'information_schema')
ORDER BY n.nspname, c.relname;
"@

$regularResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -F '|' -c $regularQuery

$regularSchemas = @{}
$tableArgs = @()

$regularResult -split "`n" | Where-Object { $_ } | ForEach-Object {
    $parts = $_ -split '\|'
    if ($parts.Length -ge 2) {
        $schema = $parts[0].Trim()
        $table = $parts[1].Trim()
        
        if (!$regularSchemas.ContainsKey($schema)) {
            $regularSchemas[$schema] = @()
        }
        $regularSchemas[$schema] += $table
        
        $tableArgs += "--table"
        $tableArgs += "$schema.$table"
    }
}

foreach ($schema in $regularSchemas.Keys | Sort-Object) {
    Write-Host "   📁 $schema" -ForegroundColor Cyan
    foreach ($table in $regularSchemas[$schema]) {
        Write-Host "      📄 $table" -ForegroundColor Green
    }
}

$regularCount = ($regularSchemas.Values | Measure-Object -Sum Count).Sum
Write-Host ""
Write-Host "   ✅ Found $regularCount regular tables" -ForegroundColor Green

# Step 2: Discover partitioned tables
Write-Host ""
Write-Host "🔍 Step 2: Discovering partitioned tables..." -ForegroundColor Yellow

$partitionedQuery = @"
SELECT n.nspname, c.relname
FROM pg_class c
JOIN pg_namespace n ON n.oid = c.relnamespace
WHERE c.relkind = 'p'
  AND NOT c.relispartition
  AND n.nspname NOT IN ('pg_catalog', 'information_schema')
ORDER BY n.nspname, c.relname;
"@

$partitionedResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -F '|' -c $partitionedQuery

$partitionedTables = @()
$partitionedResult -split "`n" | Where-Object { $_ } | ForEach-Object {
    $parts = $_ -split '\|'
    if ($parts.Length -ge 2) {
        $partitionedTables += @{
            Schema = $parts[0].Trim()
            Table  = $parts[1].Trim()
        }
        Write-Host "   📁 $($parts[0].Trim()).$($parts[1].Trim())" -ForegroundColor Magenta
    }
}

Write-Host ""
Write-Host "   ✅ Found $($partitionedTables.Count) partitioned tables" -ForegroundColor Green

$env:PGPASSWORD = $null

if (!(Test-Path $OutputDir)) {
    Write-Host "❌ Output directory not found: $OutputDir" -ForegroundColor Red
    exit 1
}

Push-Location $OutputDir

# Step 3: Scaffold regular tables with EF Core
Write-Host ""
Write-Host "🔨 Step 3: Scaffolding regular tables with EF Core..." -ForegroundColor Yellow

$conn = "Host=$DbHost;Database=$DbName;Username=$DbUser;Password=$DbPass"

$cmd = @(
    "ef", "dbcontext", "scaffold", $conn, "Npgsql.EntityFrameworkCore.PostgreSQL"
) + $tableArgs + @(
    "--output-dir", "Models",
    "--context-dir", "Data",
    "--context", $ContextName,
    "--no-pluralize"
)

if ($Force) {
    $cmd += "--force"
}

& dotnet $cmd

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ EF Core scaffolding failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host "   ✅ EF Core scaffolding complete" -ForegroundColor Green

# Step 4: Generate models for partitioned tables
Write-Host ""
Write-Host "🔨 Step 4: Generating models for partitioned tables..." -ForegroundColor Yellow

$env:PGPASSWORD = $DbPass

foreach ($pt in $partitionedTables) {
    $schema = $pt.Schema
    $table = $pt.Table
    $className = ($table -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
    
    # Get column info
    $colQuery = @"
SELECT column_name, data_type, is_nullable, character_maximum_length
FROM information_schema.columns
WHERE table_schema = '$schema' AND table_name = '$table'
ORDER BY ordinal_position;
"@
    
    $columns = psql -U $DbUser -h $DbHost -d $DbName -t -A -F '|' -c $colQuery
    
    $props = ""
    $columns -split "`n" | Where-Object { $_ } | ForEach-Object {
        $colParts = $_ -split '\|'
        if ($colParts.Length -ge 3) {
            $colName = $colParts[0].Trim()
            $dataType = $colParts[1].Trim()
            $nullable = $colParts[2].Trim() -eq "YES"
            $maxLen = if ($colParts.Length -ge 4) { $colParts[3].Trim() } else { "" }
            
            # Map PostgreSQL types to C# (using if/elseif to avoid switch regex issues)
            $csharpType = "string" # default
            if ($dataType -eq "bigint") { $csharpType = "long" }
            elseif ($dataType -match "^integer$|^int4$") { $csharpType = "int" }
            elseif ($dataType -match "^smallint$|^int2$") { $csharpType = "short" }
            elseif ($dataType -eq "boolean") { $csharpType = "bool" }
            elseif ($dataType -match "^character varying$|^varchar$|^text$") { $csharpType = "string" }
            elseif ($dataType -match "^character$|^bpchar$") { $csharpType = "string" }
            elseif ($dataType -eq "date") { $csharpType = "DateOnly" }
            elseif ($dataType -match "^timestamp") { $csharpType = "DateTime" }
            
            $nullableSuffix = ""
            if ($nullable) {
                $nullableSuffix = "?"
            }
            
            $propName = ($colName -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
            
            $lenAttr = ""
            if ($maxLen -and $csharpType -eq "string") {
                $lenAttr = "`n    [StringLength($maxLen)]"
            }
            
            $props += @"

    [Column("$colName")]$lenAttr
    public $csharpType$nullableSuffix $propName { get; set; }
"@
        }
    }
    
    $modelContent = @"
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("$table", Schema = "$schema")]
public class $className
{$props
}
"@
    
    $modelPath = "Models\$className.cs"
    $modelContent | Out-File -FilePath $modelPath -Encoding utf8
    Write-Host "   ✅ Generated $className.cs" -ForegroundColor Green
}

$env:PGPASSWORD = $null

# Step 5: Create partial DbContext for billing tables with GENERIC configuration
Write-Host ""
Write-Host "🔨 Step 5: Creating DbContext extension for partitioned tables..." -ForegroundColor Yellow

$env:PGPASSWORD = $DbPass

$dbSets = ""
$modelConfigs = ""

foreach ($pt in $partitionedTables) {
    $schema = $pt.Schema
    $table = $pt.Table
    $className = ($table -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
    $dbSets += "`n    public virtual DbSet<$className> $className { get; set; }"
    
    # Query PRIMARY KEY columns dynamically
    $pkQuery = @"
SELECT a.attname
FROM pg_index i
JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
WHERE i.indrelid = '$schema.$table'::regclass
AND i.indisprimary
ORDER BY array_position(i.indkey, a.attnum);
"@
    $pkResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -c $pkQuery
    $pkColumns = @()
    $pkResult -split "`n" | Where-Object { $_ } | ForEach-Object {
        $colName = $_.Trim()
        $propName = ($colName -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
        $pkColumns += "e.$propName"
    }
    $pkExpression = $pkColumns -join ", "
    
    # Query identity/serial columns
    $identityQuery = @"
SELECT column_name FROM information_schema.columns
WHERE table_schema = '$schema' AND table_name = '$table'
AND (is_identity = 'YES' OR column_default LIKE 'nextval%');
"@
    $identityResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -c $identityQuery
    $identityCols = @()
    $identityResult -split "`n" | Where-Object { $_ } | ForEach-Object {
        $colName = $_.Trim()
        $propName = ($colName -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
        $identityCols += $propName
    }
    
    # Query FOREIGN KEYS - use distinct and filter parent tables only
    $fkQuery = @"
SELECT DISTINCT
    kcu.column_name,
    ccu.table_name AS ref_table
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu 
    ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage ccu 
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
    AND tc.table_schema = '$schema' AND tc.table_name = '$table'
    AND ccu.table_name NOT LIKE '%_2%';
"@
    $fkResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -F '|' -c $fkQuery
    $foreignKeys = @{}
    $fkResult -split "`n" | Where-Object { $_ } | ForEach-Object {
        $parts = $_ -split '\|'
        if ($parts.Length -ge 2) {
            $colName = $parts[0].Trim()
            $refTable = $parts[1].Trim()
            $propName = ($colName -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
            $refClassName = ($refTable -split '_' | ForEach-Object { $_.Substring(0, 1).ToUpper() + $_.Substring(1) }) -join ''
            # Use hashtable to deduplicate
            $key = "$propName->$refClassName"
            if (-not $foreignKeys.ContainsKey($key)) {
                $foreignKeys[$key] = @{
                    Column   = $propName
                    RefTable = $refClassName
                }
            }
        }
    }
    
    # Query INDEXES (non-primary)
    $idxQuery = @"
SELECT indexname, indexdef
FROM pg_indexes
WHERE schemaname = '$schema' AND tablename = '$table'
AND indexname NOT LIKE '%_pkey';
"@
    $idxResult = psql -U $DbUser -h $DbHost -d $DbName -t -A -F '|' -c $idxQuery
    
    # Build entity configuration
    $entityConfig = @"

        // $className
        modelBuilder.Entity<$className>(entity =>
        {
"@
    
    # Add primary key
    if ($pkColumns.Count -gt 1) {
        $entityConfig += "`n            entity.HasKey(e => new { $pkExpression });"
    }
    elseif ($pkColumns.Count -eq 1) {
        $entityConfig += "`n            entity.HasKey(e => $($pkColumns[0]));"
    }
    
    # Add identity columns
    foreach ($idCol in $identityCols) {
        $entityConfig += "`n            entity.Property(e => e.$idCol).ValueGeneratedOnAdd();"
    }
    
    # Add foreign keys
    foreach ($fk in $foreignKeys.Values) {
        $entityConfig += "`n            entity.HasOne<$($fk.RefTable)>().WithMany().HasForeignKey(e => e.$($fk.Column));"
    }
    
    $entityConfig += "`n        });"
    $modelConfigs += $entityConfig
}

$env:PGPASSWORD = $null

$partialContext = @"
using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.Data;

/// <summary>
/// Partial class extending ApplicationDbContext with partitioned billing tables.
/// This file is AUTO-GENERATED by scaffold-models.ps1 - do not edit manually.
/// 
/// WHY PARTIAL CLASS?
/// - EF Core scaffolding cannot handle PostgreSQL partitioned tables
/// - When scaffolding runs, it regenerates ApplicationDbContext.cs but NOT this file
/// - This allows us to maintain billing tables while keeping scaffolding for regular tables
/// </summary>
public partial class $ContextName
{
    // DbSets for partitioned billing tables$dbSets

    /// <summary>
    /// Configure billing tables - PKs, FKs, indexes auto-generated from database schema
    /// Called from main OnModelCreating via OnModelCreatingPartial
    /// </summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {$modelConfigs
    }
}
"@

$partialContext | Out-File -FilePath "Data\$ContextName.Billing.cs" -Encoding utf8
Write-Host "   ✅ Generated $ContextName.Billing.cs (with PKs, FKs from database)" -ForegroundColor Green

# Step 6: Build to verify
Write-Host ""
Write-Host "🔨 Step 6: Building to verify..." -ForegroundColor Yellow
dotnet build --nologo -v q

if ($LASTEXITCODE -eq 0) {
    $modelCount = (Get-ChildItem Models\*.cs -ErrorAction SilentlyContinue | Measure-Object).Count
    Write-Host ""
    Write-Host "========================================================================" -ForegroundColor Green
    Write-Host " ✅ SCAFFOLDING COMPLETE!" -ForegroundColor Green
    Write-Host "========================================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📁 Generated:" -ForegroundColor Cyan
    Write-Host "   Models\ - $modelCount files" -ForegroundColor White
    Write-Host "   Data\$ContextName.cs ✓" -ForegroundColor Green
    Write-Host "   Data\$ContextName.Billing.cs ✓ (partitioned tables)" -ForegroundColor Green
    Write-Host ""
}
else {
    Write-Host ""
    Write-Host "⚠️  Build has warnings/errors - check output" -ForegroundColor Yellow
}

Pop-Location
pause
