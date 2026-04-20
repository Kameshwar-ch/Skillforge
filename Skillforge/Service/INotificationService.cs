namespace Skillforge.Service;

/// <summary>
/// Defines the notification contract for system-generated events.
/// Implementations may dispatch emails, push messages, or audit entries.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifies an employee that their certification has been issued.
    /// </summary>
    /// <param name="employeeId">The recipient employee's user ID.</param>
    /// <param name="certificationId">The ID of the newly issued certification.</param>
    Task NotifyCertificationIssuedAsync(int employeeId, int certificationId);

    /// <summary>
    /// Notifies the admin that a scheduled report has been generated.
    /// </summary>
    /// <param name="adminId">The admin user ID who owns the schedule.</param>
    /// <param name="reportId">The ID of the generated report.</param>
    /// <param name="scope">The scope of the report.</param>
    Task NotifyReportGeneratedAsync(int adminId, int reportId, string scope);
}
