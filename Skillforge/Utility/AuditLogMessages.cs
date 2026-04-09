namespace Skillforge.Utility
{
    /// <summary>
    /// Centralized messages for immutable AuditLog operations.
    /// Provides standardized text for controller/service responses.
    /// </summary>
    public static class AuditLogMessages
    {
        public const string NotFound = "Audit log not found.";
        public const string NoLogs = "No audit logs available.";
        public const string InvalidId = "Invalid audit log ID.";
        public const string InvalidPagination = "Page and PageSize must be greater than zero.";
        public const string Error = "An unexpected error occurred while retrieving audit logs.";
        public const string Success = "Audit logs retrieved successfully.";
    }
}
