namespace Skillforge.Dto
{
    /// <summary>
    /// Data Transfer Object representing an immutable audit log entry.
    /// </summary>
    public class AuditLogDto
    {
        public int AuditID { get; set; }
        public int? UserID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Generic wrapper for paginated results.
    /// </summary>
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
