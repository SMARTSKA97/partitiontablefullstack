namespace PartitionTableFullStack.API.Common;

public class FilterCriteria
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class SortCriteria
{
    public string Field { get; set; } = string.Empty;
    public string Order { get; set; } = "asc";
}

public class QueryParameters
{
    public string? GlobalSearch { get; set; }
    public List<FilterCriteria> Filters { get; set; } = new List<FilterCriteria>();
    public List<SortCriteria> Sorts { get; set; } = new List<SortCriteria>();
    public List<string> SelectColumns { get; set; } = new();
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<string> IncludeProperties { get; set; } = new List<string>();
}

public class PaginatedResponseObject<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, object> MetaData { get; set; } = new();
}
