# Database Setup Guide

## Prerequisites
- PostgreSQL 16+ installed
- pgAdmin 4 (optional, for GUI)

## Setup Steps

1. **Create Database**
   ```sql
   CREATE DATABASE billing_system;
   ```

2. **Execute Scripts in Order**
   
   Run the scripts in the following sequence:
   
   ```bash
   psql -U postgres -d billing_system -f 01-init-schemas.sql
   psql -U postgres -d billing_system -f 02-master-tables.sql
   psql -U postgres -d billing_system -f 03-billing-master-tables.sql
   psql -U postgres -d billing_system -f 04-partitioned-tables.sql
   psql -U postgres -d billing_system -f 05-functions-and-procedures.sql
   psql -U postgres -d billing_system -f 06-seed-data.sql
   ```

## Verify Setup

```sql
-- Check partitions
SELECT 
    schemaname, 
    tablename, 
    partitionschemaname,
    partitiontablename
FROM pg_partitions 
WHERE schemaname = 'billing'
ORDER BY tablename, partitiontablename;

-- Check financial years
SELECT * FROM master.financial_year_master ORDER BY id;

-- Verify sample data
SELECT COUNT(*) FROM master.treasury;
SELECT COUNT(*) FROM master.ddo;
SELECT COUNT(*) FROM billing_master.bill_status_master;
```

## Partition Strategy

- All billing tables are partitioned by `financial_year` (foreign key to `master.financial_year_master.id`)
- Partitions are created for FY 2425 (ID=1), 2526 (ID=2), 2627 (ID=3)
- Only bills for active FY (currently 2024-2025) can be created via application
- Old FY data is read-only from application level
- Database admins can directly modify partitions if needed
