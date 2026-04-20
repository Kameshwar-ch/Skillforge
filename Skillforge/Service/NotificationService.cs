namespace Skillforge.Service;

/// <summary>
/// Writes certification notification events to the audit log.
/// Replace or extend this implementation to add email/SMS delivery.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IAuditService _auditService;

    public NotificationService(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task NotifyCertificationIssuedAsync(int employeeId, int certificationId)
    {
        await _auditService.LogAsync(
            employeeId,
            "CertificationIssued",
            $"Certification/{certificationId}");
    }

    public async Task NotifyReportGeneratedAsync(int adminId, int reportId, string scope)
    {
        await _auditService.LogAsync(
            adminId,
            "ScheduledReportGenerated",
            $"Report/{reportId}/Scope/{scope}");
    }
}
