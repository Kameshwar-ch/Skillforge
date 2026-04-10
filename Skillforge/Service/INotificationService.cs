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
}
