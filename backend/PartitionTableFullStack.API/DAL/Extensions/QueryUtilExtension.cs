using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace PartitionTableFullStack.API.DAL.Extensions;

public static class QueryUtilExtension
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, IEnumerable<FilterCriteria> filters)
    {
        if (filters == null || !filters.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var filter in filters)
        {
            // Handle jsoncontains before touching Expression.Property
            if (filter.Operator.ToLower() == "jsoncontains")
            {
                var parts = filter.Field.Split('.');
                if (parts.Length == 2)
                {
                    var jsonColumn = parts[0];
                    var jsonKey = parts[1];

                    query = query.Where(e =>
                        EF.Functions.JsonContains(EF.Property<string>(e, jsonColumn),
                        $@"{{""{jsonKey}"": {filter.Value}}}") ||
                        EF.Functions.JsonContains(EF.Property<string>(e, jsonColumn),
                        $@"{{""{jsonKey}"": ""{filter.Value}""}}")
                    );
                    continue;
                }
                throw new ArgumentException("Invalid format for 'jsoncontains'. Expected format: 'PropertyName.JsonKey'");
            }

            var property = Expression.Property(parameter, filter.Field);
            switch (filter.Operator.ToLower())
            {
                case "eq":
                    if (DateOnly.TryParse(filter.Value, out var date))
                    {
                        query = query.Where(e => EF.Property<DateOnly>(e, filter.Field) == date);
                    }
                    else if (bool.TryParse(filter.Value, out var boolValue))
                    {
                        query = query.Where(e => EF.Property<bool>(e, filter.Field) == boolValue);
                    }
                    else if (property.Type == typeof(string))
                    {
                        query = query.Where(e => EF.Property<string>(e, filter.Field) == filter.Value);
                    }
                    else if (long.TryParse(filter.Value, out var longValue))
                    {
                        query = query.Where(e => EF.Property<long>(e, filter.Field) == longValue);
                    }
                    else
                    {
                        query = query.Where(e => EF.Property<string>(e, filter.Field) == filter.Value);
                    }
                    break;

                case "between":
                    var values = filter.Value.Split(',');
                    if (values.Length == 2)
                    {
                        if (DateOnly.TryParse(values[0], out var startDateOnly) && DateOnly.TryParse(values[1], out var endDateOnly))
                        {
                            query = query.Where(e => EF.Property<DateOnly>(e, filter.Field) >= startDateOnly && EF.Property<DateOnly>(e, filter.Field) <= endDateOnly);
                        }
                        else if (DateTime.TryParse(values[0], out var startDateTime) && DateTime.TryParse(values[1], out var endDateTime))
                        {
                            query = query.Where(e => EF.Property<DateTime>(e, filter.Field) >= startDateTime && EF.Property<DateTime>(e, filter.Field) <= endDateTime);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid format for 'between' operator. Expected format: 'start,end' with DateOnly or DateTime.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid format for 'between' operator. Expected format: 'start,end'.");
                    }
                    break;

                case "gt":
                    if (DateTime.TryParse(filter.Value, out var dateTimeGt))
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, filter.Field) > dateTimeGt);
                    }
                    else if (decimal.TryParse(filter.Value, out var decimalGt))
                    {
                        query = query.Where(e => EF.Property<decimal>(e, filter.Field) > decimalGt);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid format for 'gt' operator: {filter.Value}");
                    }
                    break;

                case "gte":
                    if (DateTime.TryParse(filter.Value, out var dateTimeGte))
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, filter.Field) >= dateTimeGte);
                    }
                    else if (decimal.TryParse(filter.Value, out var decimalGte))
                    {
                        query = query.Where(e => EF.Property<decimal>(e, filter.Field) >= decimalGte);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid format for 'gte' operator: {filter.Value}");
                    }
                    break;

                case "lt":
                    if (DateTime.TryParse(filter.Value, out var dateTimeLt))
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, filter.Field) < dateTimeLt);
                    }
                    else if (decimal.TryParse(filter.Value, out var decimalLt))
                    {
                        query = query.Where(e => EF.Property<decimal>(e, filter.Field) < decimalLt);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid format for 'lt' operator: {filter.Value}");
                    }
                    break;

                case "lte":
                    if (DateTime.TryParse(filter.Value, out var dateTimeLte))
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, filter.Field) <= dateTimeLte);
                    }
                    else if (decimal.TryParse(filter.Value, out var decimalLte))
                    {
                        query = query.Where(e => EF.Property<decimal>(e, filter.Field) <= decimalLte);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid format for 'lte' operator: {filter.Value}");
                    }
                    break;

                case "contains":
                    query = query.Where(e => EF.Property<string>(e, filter.Field).Contains(filter.Value));
                    break;

                case "isnull":
                    query = query.Where(e => EF.Property<string>(e, filter.Field) == null);
                    break;

                case "isnotnull":
                    query = query.Where(e => EF.Property<string>(e, filter.Field) != null);
                    break;

                case "startswith":
                    if (property.Type == typeof(long) || property.Type == typeof(int))
                    {
                        query = query.Where(e => EF.Property<long>(e, filter.Field).ToString().StartsWith(filter.Value));
                    }
                    else
                    {
                        query = query.Where(e => EF.Property<string>(e, filter.Field).StartsWith(filter.Value));
                    }
                    break;

                case "ne":
                    query = query.Where(e => EF.Property<string>(e, filter.Field) != filter.Value);
                    break;

                default:
                    throw new NotSupportedException($"The operator '{filter.Operator}' is not supported.");
            }
        }
        return query;
    }

    public static IQueryable<T> ApplySorts<T>(this IQueryable<T> query, IEnumerable<SortCriteria> sorts)
    {
        if (sorts == null || !sorts.Any())
            return query;

        IOrderedQueryable<T>? orderedQuery = null;
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var (sort, index) in sorts.Select((s, i) => (s, i)))
        {
            var property = Expression.Property(parameter, sort.Field);
            var lambda = Expression.Lambda(property, parameter);
            
            var methodName = index == 0
                ? (sort.Order.ToLower() == "desc" ? "OrderByDescending" : "OrderBy")
                : (sort.Order.ToLower() == "desc" ? "ThenByDescending" : "ThenBy");

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery ?? query, lambda })!;
        }

        return orderedQuery ?? query;
    }

    public static IQueryable<T> ApplyGlobalSearch<T>(this IQueryable<T> source, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;
        var searchTermLower = Expression.Constant(searchTerm.ToLower());

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });

        var dateFormats = new[] { "yyyy-MM-dd", "dd-MM-yyyy", "MM/dd/yyyy", "yyyy/MM/dd" };

        foreach (var prop in typeof(T).GetProperties())
        {
            // Skip JSONB properties
            if (prop.GetCustomAttribute<ColumnAttribute>()?.TypeName?.ToLower() == "jsonb")
                continue;

            // Handle string properties
            if (prop.PropertyType == typeof(string))
            {
                var propertyAccess = Expression.Property(parameter, prop);
                var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var propertyLower = Expression.Call(propertyAccess, toLowerMethod!);
                var contains = Expression.Call(propertyLower, containsMethod!, searchTermLower);
                var condition = Expression.AndAlso(notNull, contains);
                combined = combined == null ? condition : Expression.OrElse(combined, condition);
            }
            // Handle boolean properties
            else if (prop.PropertyType == typeof(bool))
            {
                if (bool.TryParse(searchTerm, out bool searchBool))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var equals = Expression.Equal(propertyAccess, Expression.Constant(searchBool));
                    combined = combined == null ? equals : Expression.OrElse(combined, equals);
                }
            }
            // Handle nullable boolean properties
            else if (prop.PropertyType == typeof(bool?))
            {
                if (bool.TryParse(searchTerm, out bool searchBool))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(bool?)));
                    var equals = Expression.Equal(propertyAccess, Expression.Constant(searchBool, typeof(bool?)));
                    var condition = Expression.AndAlso(notNull, equals);
                    combined = combined == null ? condition : Expression.OrElse(combined, condition);
                }
            }
            // Handle numeric properties
            else if (IsNumericType(prop.PropertyType))
            {
                if (TryParseNumeric(searchTerm, prop.PropertyType, out object? numericValue))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var equals = Expression.Equal(propertyAccess, Expression.Constant(numericValue, prop.PropertyType));
                    combined = combined == null ? equals : Expression.OrElse(combined, equals);
                }
            }
            // Handle DateOnly properties
            else if (prop.PropertyType == typeof(DateOnly))
            {
                if (TryParseDate(searchTerm, dateFormats, out DateOnly searchDate))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var equals = Expression.Equal(propertyAccess, Expression.Constant(searchDate));
                    combined = combined == null ? equals : Expression.OrElse(combined, equals);
                }
            }
            // Handle DateOnly? properties
            else if (prop.PropertyType == typeof(DateOnly?))
            {
                if (TryParseDate(searchTerm, dateFormats, out DateOnly searchDate))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(DateOnly?)));
                    var equals = Expression.Equal(propertyAccess, Expression.Constant(searchDate, typeof(DateOnly?)));
                    var condition = Expression.AndAlso(notNull, equals);
                    combined = combined == null ? condition : Expression.OrElse(combined, condition);
                }
            }
            // Handle DateTime properties
            else if (prop.PropertyType == typeof(DateTime))
            {
                if (DateTime.TryParse(searchTerm, out DateTime searchDateTime))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var datePart = Expression.Property(propertyAccess, nameof(DateTime.Date));
                    var equals = Expression.Equal(datePart, Expression.Constant(searchDateTime.Date));
                    combined = combined == null ? equals : Expression.OrElse(combined, equals);
                }
            }
            // Handle DateTime? properties
            else if (prop.PropertyType == typeof(DateTime?))
            {
                if (DateTime.TryParse(searchTerm, out DateTime searchDateTime))
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(DateTime?)));
                    var dateValue = Expression.Property(propertyAccess, nameof(Nullable<DateTime>.Value));
                    var datePart = Expression.Property(dateValue, nameof(DateTime.Date));
                    var equals = Expression.Equal(datePart, Expression.Constant(searchDateTime.Date));
                    var condition = Expression.AndAlso(notNull, equals);
                    combined = combined == null ? condition : Expression.OrElse(combined, condition);
                }
            }
        }

        return combined == null
            ? source
            : source.Where(Expression.Lambda<Func<T, bool>>(combined, parameter));
    }

    private static bool TryParseDate(string dateString, string[] formats, out DateOnly result)
    {
        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            result = DateOnly.FromDateTime(dateTime);
            return true;
        }
        result = default;
        return false;
    }

    private static bool IsNumericType(Type type)
    {
        var numericTypes = new[]
        {
            typeof(int), typeof(int?),
            typeof(long), typeof(long?),
            typeof(decimal), typeof(decimal?),
            typeof(double), typeof(double?),
            typeof(float), typeof(float?),
            typeof(short), typeof(short?),
            typeof(byte), typeof(byte?)
        };

        return numericTypes.Contains(type);
    }

    private static bool TryParseNumeric(string input, Type targetType, out object? result)
    {
        result = null;

        try
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(int))
            {
                if (int.TryParse(input, out int intValue))
                {
                    result = intValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(long))
            {
                if (long.TryParse(input, out long longValue))
                {
                    result = longValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(decimal))
            {
                if (decimal.TryParse(input, out decimal decimalValue))
                {
                    result = decimalValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(double))
            {
                if (double.TryParse(input, out double doubleValue))
                {
                    result = doubleValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(float))
            {
                if (float.TryParse(input, out float floatValue))
                {
                    result = floatValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(short))
            {
                if (short.TryParse(input, out short shortValue))
                {
                    result = shortValue;
                    return true;
                }
            }
            else if (underlyingType == typeof(byte))
            {
                if (byte.TryParse(input, out byte byteValue))
                {
                    result = byteValue;
                    return true;
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return false;
    }
}
