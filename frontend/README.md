# Angular Frontend

Angular 18 standalone application for the billing system.

## Quick Start

```bash
# Install dependencies
npm install

# Start development server
ng serve

# Build for production
ng build
```

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── bill-create/
│   │   ├── bill-list/
│   │   ├── bill-details/
│   │   └── report/
│   └── services/
│       ├── bill.service.ts
│       └── report.service.ts
├── environments/
│   └── environment.ts
└── main.ts
```

## Components

### Bill Create Component
- Form with reactive validations
- Dynamic arrays for BT, GST, ECS, Allotment, Subvoucher
- GST toggle (optional)
- Auto financial year detection from bill date

### Bill List Component
- Financial year dropdown filter
- Search/filter functionality
- Pagination
- Navigate to details

### Bill Details Component
- Display all bill information
- Show related components in tabs/sections

### Report Component
- Financial year selector
- Export buttons (Excel/PDF)
- File download handling

## Dependencies

```json
{
  "@angular/core": "^18.0.0",
  "@angular/forms": "^18.0.0",
  "@angular/common": "^18.0.0",
  "@angular/router": "^18.0.0",
  "primeng": "^17.x",
  "primeicons": "^7.x",
  "rxjs": "^7.x"
}
```

## Configuration

Update API URL in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7123/api'
};
```

## Running

Default: http://localhost:4200/

Routes:
- `/` - Home/Dashboard
- `/bills/create` - Create bill form
- `/bills` - Bill list
- `/bills/:id` - Bill details
- `/reports` - Report generation

## Notes

- Components use standalone configuration (Angular 18)
- PrimeNG for UI components
- Reactive forms for validation
- RxJS for async operations
