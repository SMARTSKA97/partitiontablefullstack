# Partition Table Full Stack Project

A comprehensive billing system built with PostgreSQL partitioned tables (by financial year), .NET 8 Web API backend, and Angular 18 frontend.

## Project Overview

This system implements a multi-year bill management solution where bills and their related components (BT, GST, ECS, Allotment, Subvoucher) are partitioned by financial year (April 1 - March 31). Old financial year data becomes read-only at the application level, while current FY data remains fully editable.

## Technology Stack

- **Database**: PostgreSQL 16+ with native table partitioning
- **Backend**: .NET 8 (LTS) Web API with EF Core 8
- **Frontend**: Angular 18 (LTS) with PrimeNG UI components
- **Reports**: Excel (EPPlus), PDF (QuestPDF)

## Project Structure

```
partitiontablefullstack/
├── database/
│   ├── scripts/
│   │   ├── 01-init-schemas.sql
│   │   ├── 02-master-tables.sql
│   │   ├── 03-billing-master-tables.sql
│   │   ├── 04-partitioned-tables.sql
│   │   ├── 05-functions-and-procedures.sql
│   │   └── 06-seed-data.sql
│   └── README.md
├── backend/
│   ├── PartitionTableFullStack.sln
│   └── PartitionTableFullStack.API/
│       ├── Controllers/
│       ├── Services/
│       ├── Models/
│       ├── DTOs/
│       ├── Data/
│       └── appsettings.json
└── frontend/
    ├── src/
    │   ├── app/
    │   │   ├── components/
    │   │   └── services/
    │   └── environments/
    └── package.json
```

## Prerequisites

### Required Software

1. **PostgreSQL 16+**
   - Download from: https://www.postgresql.org/download/
   - Ensure server is running on port 5432

2. **.NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify: `dotnet --version`

3. **Node.js 20+ LTS**
   - Download from: https://nodejs.org/
   - Verify: `node --version` and `npm --version`

4. **Angular CLI 18**
   - Install: `npm install -g @angular/cli@18`
   - Verify: `ng version`

## Setup Instructions

### 1. Database Setup

```bash
cd database/scripts

# Create database
psql -U postgres -c "CREATE DATABASE billing_system;"

# Execute scripts in order
psql -U postgres -d billing_system -f 01-init-schemas.sql
psql -U postgres -d billing_system -f 02-master-tables.sql
psql -U postgres -d billing_system -f 03-billing-master-tables.sql
psql -U postgres -d billing_system -f 04-partitioned-tables.sql
psql -U postgres -d billing_system -f 05-functions-and-procedures.sql
psql -U postgres -d billing_system -f 06-seed-data.sql

# Verify
psql -U postgres -d billing_system -c "SELECT * FROM master.financial_year_master;"
```

### 2. Backend Setup

```bash
cd backend/PartitionTableFullStack.API

# Update connection string in appsettings.json if needed
# Default: "Host=localhost;Port=5432;Database=billing_system;Username=postgres;Password=postgres"

# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

Backend will be available at:
- **Swagger UI**: https://localhost:7123/ (or configured port)
- **API Base**: https://localhost:7123/api/

### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Update API URL in src/environments/environment.ts if needed
# Default: http://localhost:5123/api

# Start dev server
ng serve

# Or start on specific port
ng serve --port 4200
```

Frontend will be available at: http://localhost:4200/

## API Endpoints

### Bills API (`/api/bills`)

- **POST** `/api/bills` - Create bill with components
- **GET** `/api/bills?financialYear={fy}&search={term}&page={n}&pageSize={size}` - Get bill list
- **GET** `/api/bills/{id}` - Get bill details
- **GET** `/api/bills/financial-years` - Get available financial years

### Reports API (`/api/reports`)

- **GET** `/api/reports/excel?financialYear={fy}` - Download Excel report
- **GET** `/api/reports/pdf?financialYear={fy}` - Download PDF report

## Features

### Bill Management

1. **Bill Creation**
   - Create bills with auto financial year detection based on bill date
   - Add multiple BT, GST, ECS, Allotment, Subvoucher entries
   - GST is optional (toggle-based)
   - Automatic bill number generation (format: FY2425-00001)
   - All operations via PostgreSQL stored procedure

2. **Bill List View**
   - Filter by financial year (dropdown)
   - Global search (bill number, DDO code, remarks)
   - Pagination
   - Click to view details

3. **Bill Details**
   - Comprehensive view of bill and all sub-components
   - Organized layout with sections

4. **Bill Reports**
   - Summary report by financial year
   - Download as Excel or PDF
   - Includes totals and individual bill listings

### Partition Strategy

- Bills are partitioned by `financial_year` foreign key
- Current partition: `bill_details_2425` (FY 2024-2025, ID=1)
- Future partitions: `bill_details_2526`, `bill_details_2627`
- Read-only enforcement: Only active FY (is_active=true) allows bill creation
- Old FYs are read-only from application level

## Database Schema

### Key Tables

- `master.financial_year_master` - Financial year configuration
- `billing.bill_details` - Main bill table (partitioned)
- `billing.bill_btdetail` - BT details (partitioned)
- `billing.bill_gst` - GST/CPIN details (partitioned)
- `billing.bill_ecs_neft_details` - Payment beneficiary details (partitioned)
- `billing.ddo_allotment_booked_bill` - Budget allotment (partitioned)
- `billing.bill_subvoucher` - Subvoucher details (partitioned)

### Financial Year Logic

- FY runs from April 1 to March 31
- Date 2024-05-15 → FY 2024-2025 (ID=1)
- Date 2024-02-10 → FY 2023-2024
- Bill dates automatically determine which partition

## Development

### Adding a New Financial Year

1. Insert into `master.financial_year_master`:
   ```sql
   INSERT INTO master.financial_year_master (financial_year, is_active)
   VALUES ('2027-2028', false);
   ```

2. Create new partitions:
   ```sql
   CREATE TABLE billing.bill_details_2728 PARTITION OF billing.bill_details FOR VALUES IN (4);
   CREATE TABLE billing.bill_btdetail_2728 PARTITION OF billing.bill_btdetail FOR VALUES IN (4);
   -- Repeat for all partitioned tables
   ```

3. When FY becomes active, set `is_active = true` for that FY and `false` for previous FY.

### Testing

1. **Database Verification**
   ```sql
   -- Check partitions
   SELECT * FROM pg_partitions WHERE schemaname = 'billing';
   
   -- Test bill creation
   SELECT billing.insert_bill_with_components(...);
   ```

2. **API Testing**
   - Use Swagger UI at https://localhost:7123/
   - Test POST /api/bills with sample JSON
   - Verify data in database

3. **Frontend Testing**
   - Navigate to http://localhost:4200/
   - Test bill creation form
   - Verify data appears in bill list

## Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL is running: `pg_isready`
- Check connection string in `appsettings.json`
- Verify user has permissions

### Backend Build Errors
- Ensure .NET 8 SDK is installed
- Run `dotnet restore`
- Check for missing packages

### Frontend Issues
- Clear npm cache: `npm cache clean --force`
- Delete `node_modules` and reinstall: `rm -rf node_modules && npm install`
- Check Angular CLI version: `ng version`

## License

This is a demo/educational project for demonstrating PostgreSQL partition tables with .NET and Angular.

## Notes

- **Security**: Current implementation has minimal authentication (hardcoded user ID). Production apps should implement proper auth.
- **Performance**: Partitioning improves query performance for large datasets. Ensure proper indexing.
- **Maintenance**: Database admin can directly modify old FY partitions if needed.
- **Reports**: EPPlus uses NonCommercial license, QuestPDF uses Community license. Review licensing for commercial use.
