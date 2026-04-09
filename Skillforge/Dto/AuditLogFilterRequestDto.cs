namespace Skillforge.Dto
{
    public class AuditLogFilterRequestDto
    {
        // Filter conditions
        public int? AuditID { get; set; }
        public int? UserID { get; set; }
        public string? Resource { get; set; }
        public string? Action { get; set; }
        public DateTime? Timestamp { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string SortBy { get; set; } = string.Empty;
        public string SortOrder { get; set; } = string.Empty;
    }
}
