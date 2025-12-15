# 3-Layer Architecture Documentation

## Overview

The backend has been refactored to follow a clean 3-layer architecture pattern with advanced query capabilities.

## Folder Structure

```
PartitionTableFullStack.API/
├── Controllers/              # Presentation Layer - API endpoints
│   ├── BillsController.cs
│   └── ReportsController.cs
├── BLL/                      # Business Logic Layer
│   └── Services/
│       └── BillService.cs   # Business operations
├── DAL/                      # Data Access Layer
│   ├── Repositories/
│   │   ├── IRepository.cs   # Generic repository interface
│   │   ├── Repository.cs    # Generic repository implementation
│   │   ├── IBillRepository.cs
│   │   └── BillRepository.cs
│   └── Extensions/
│       └── QueryUtilExtension.cs  # Query filtering, sorting, search
├── Common/                   # Shared types
│   ├── QueryParameters.cs   # Filter, Sort, Pagination classes
│   └── ServiceResponse.cs   # Standard response wrapper
├── Models/                   # Entity models
├── DTOs/                     # Data transfer objects  
└── Data/                     # DbContext
```

## Layer Responsibilities

### 1. **Controllers Layer** (Presentation)
- Handle HTTP requests/responses
- Input validation
- Call business logic services
- Return appropriate HTTP status codes

### 2. **BLL (Business Logic Layer)**
- Business rules and validation
- Orchestrate operations
- Call repositories for data access
- Return ServiceResponse<T>

### 3. **DAL (Data Access Layer)**
- Database operations
- Query building with filters/sorts
- Repository pattern implementation
- EF Core queries

## Key Components

### QueryParameters

Advanced query system supporting:

```csharp
public class QueryParameters
{
    public string? GlobalSearch { get; set; }          // Search across all string/numeric fields
    public List<FilterCriteria> Filters { get; set; }  // Field-specific filters
    public List<SortCriteria> Sorts { get; set; }      // Multi-column sorting
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<string> IncludeProperties { get; set; } // Related entities
}
```

### FilterCriteria Operators

- `eq` - Equals
- `ne` - Not equals
- `gt`, `gte`, `lt`, `lte` - Comparisons
- `contains` - String contains
- `startswith` - String starts with
- `between` - Range (dates, numbers)
- `isnull`, `isnotnull` - Null checks
- `jsoncontains` - JSONB field search (PostgreSQL)

### ServiceResponse<T>

Standardized API responses:

```csharp
public class ServiceResponse<T>
{
    public T? Result { get; set; }
    public APIResponseStatus ApiResponseStatus { get; set; }  // Success, Error, NotFound, etc.
    public string Message { get; set; }
    public ICollection<ValidationResult> ValidationResults { get; set; }
}
```

## Usage Examples

### 1. **Simple Query with Global Search**

```http
POST /api/bills/query?financialYear=1
Content-Type: application/json

{
  "globalSearch": "CAFPNA001",
  "pageNumber": 1,
  "pageSize": 20
}
```

### 2. **Advanced Filtering**

```http
POST /api/bills/query?financialYear=1
Content-Type: application/json

{
  "filters": [
    {
      "field": "GrossAmount",
      "operator": "gt",
      "value": "10000"
    },
    {
      "field": "BillDate",
      "operator": "between",
      "value": "2024-01-01,2024-12-31"
    },
    {
      "field": "DdoCode",
      "operator": "startswith",
      "value": "CAF"
    }
  ],
  "sorts": [
    {
      "field": "BillDate",
      "order": "desc"
    }
  ],
  "pageNumber": 1,
  "pageSize": 50
}
```

### 3. **Global Search Features**

Global search automatically searches across:
- All string fields (case-insensitive contains)
- Numeric fields (exact match)
- Boolean fields (true/false)
- Date fields (multiple formats supported)

## Benefits

✅ **Separation of Concerns**: Each layer has a single responsibility
✅ **Testability**: Easy to unit test services and repositories
✅ **Maintainability**: Changes isolated to specific layers
✅ **Flexibility**: Advanced query capabilities without hardcoding logic
✅ **Consistency**: ServiceResponse wrapper for all API responses
✅ **Reusability**: Generic repository pattern for all entities

## Migration from Old Code

### Old Approach
```csharp
// Controller directly called service
var result = await _billService.GetBillsAsync(fy, search, page, pageSize);
return Ok(result);
```

### New Approach
```csharp
// Controller calls service with QueryParameters
var queryParams = new QueryParameters { GlobalSearch = search, PageNumber = page };
var result = await _billService.GetBillsAsync(fy, queryParams);
return result.ApiResponseStatus == APIResponseStatus.Success ? Ok(result) : BadRequest(result);
```

## Adding a New Entity

1. **Create Model** in `Models/`
2. **Create Repository Interface** in `DAL/Repositories/I{Entity}Repository.cs`
3. **Create Repository** in `DAL/Repositories/{Entity}Repository.cs`
4. **Create Service Interface** in `BLL/Services`
5. **Create Service** in `BLL/Services`
6. **Create Controller** in `Controllers/`
7. **Register in Program.cs** DI container

## Performance Considerations

- **Query Optimization**: Filters applied at database level
- **Pagination**: Server-side pagination reduces data transfer
- **Include Properties**: Only load related entities when needed
- **Indexing**: Create database indexes on frequently filtered columns

## Future Enhancements

- [ ] Caching layer (Redis/MemoryCache)
- [ ] Logging/Auditing middleware
- [ ] Authorization/Authentication filters
- [ ] API versioning
- [ ] GraphQL endpoint
