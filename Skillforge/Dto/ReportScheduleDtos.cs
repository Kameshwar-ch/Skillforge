using Skillforge.Domain;

namespace Skillforge.Dto;

public class CreateReportScheduleDto
{
    public ReportScope Scope { get; set; }

    /// <summary>
    /// Standard 5-field cron expression, e.g. "0 9 * * 1" = every Monday at 9 AM.
    /// </summary>
    public string CronExpression { get; set; }
}

public class ReportScheduleResponseDto
{
    public int ScheduleID { get; set; }
    public ReportScope Scope { get; set; }
    public string CronExpression { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? NextRun { get; set; }
    public bool IsActive { get; set; }
}
