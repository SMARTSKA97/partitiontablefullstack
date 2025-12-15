# Billing Frontend - Angular 18

Complete Angular 18 application for the partition table billing system.

## Features Implemented

✅ **Bill Create Component**
- Reactive forms with validation
- Dynamic form arrays for BT, GST, ECS, Allotment, Subvoucher
- GST toggle (optional)
- PrimeNG UI components

✅ **Bill List Component**
- Financial year dropdown filter
- Global search functionality
- Paginated table with PrimeNG DataTable
- Navigate to bill details

✅ **Bill Details Component**
- Tabbed view for bill information
- Separate tabs for BT, GST, ECS, Allotment, Subvoucher
- Comprehensive data display

✅ **Report Component**
- Financial year selector
- Excel download
- PDF download
- File naming with FY and date

## Quick Start

```bash
# Install dependencies (if not done)
npm install

# Start development server
ng serve

# Build for production
ng build
```

## Access

- Local: http://localhost:4200/
- Bills List: http://localhost:4200/bills
- Create Bill: http://localhost:4200/bills/create
- Reports: http://localhost:4200/reports

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── bill-create/          ✓ Complete
│   │   ├── bill-list/            ✓ Complete
│   │   ├── bill-details/         ✓ Complete
│   │   └── report/               ✓ Complete
│   ├── services/
│   │   ├── bill.service.ts       ✓ Complete
│   │   └── report.service.ts     ✓ Complete
│   ├── app.component.ts          ✓ Complete
│   ├── app.config.ts             ✓ Complete
│   └── app.routes.ts             ✓ Complete
├── environments/
│   └── environment.ts            ✓ Complete
└── styles.scss                   ✓ Complete with PrimeNG
```

## Dependencies

- Angular 18.x
- PrimeNG 17.x - UI Components
- PrimeIcons 7.x - Icons
- PrimeFlex - CSS Utilities

## Configuration

Update backend API URL in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7123/api'
};
```

## Running Tests

```bash
npm test
```

## Build

```bash
npm run build
```

## Notes

- Uses standalone components (Angular 18 pattern)
- Reactive forms throughout
- HttpClient for API communication
- PrimeNG for consistent UI
- SCSS for styling
