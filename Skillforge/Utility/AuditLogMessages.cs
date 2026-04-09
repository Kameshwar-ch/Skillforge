namespace Skillforge.Utility
{
    public static class AuditLogMessages
    {
        public const string NotFound = "Audit log not found.";
        public const string NoLogs = "No audit logs available.";
        public const string InvalidId = "Invalid audit log ID.";
        public const string InvalidPagination = "Page and PageSize must be greater than zero.";
        public const string InvalidSortBy = "Invalid sort field. Allowed: AuditID, UserID, Resource, Action, Timestamp.";
        public const string InvalidSortOrder = "Invalid sort order. Allowed: asc, desc.";
        public const string Error = "An unexpected error occurred while retrieving audit logs.";
        public const string Success = "Audit logs retrieved successfully.";
    }
}
