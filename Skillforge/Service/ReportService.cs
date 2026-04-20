using Cronos;
using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
using System.Text.Json;

namespace Skillforge.Service;

public class ReportService : IReportService
{
    private static readonly TimeZoneInfo IstZone =
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

    private static DateTime NowIst() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IstZone);

    private readonly IReportRepository _reportRepository;
    private readonly INotificationService _notificationService;
    private readonly SkillForgeDB _context;

    public ReportService(
        IReportRepository reportRepository,
        INotificationService notificationService,
        SkillForgeDB context)
    {
        _reportRepository = reportRepository;
        _notificationService = notificationService;
        _context = context;
    }

    public async Task<ReportScheduleResponseDto> CreateScheduleAsync(CreateReportScheduleDto dto, int adminId)
    {
        var cron = CronExpression.Parse(dto.CronExpression);
        var nextRun = cron.GetNextOccurrence(DateTime.UtcNow, IstZone);

        var schedule = new ReportSchedule
        {
            Scope = dto.Scope,
            CronExpression = dto.CronExpression,
            CreatedBy = adminId,
            CreatedAt = NowIst(),
            NextRun = nextRun,
            IsActive = true
        };

        await _reportRepository.CreateScheduleAsync(schedule);

        return MapToDto(schedule);
    }

    public async Task<IEnumerable<ReportScheduleResponseDto>> GetAllSchedulesAsync()
    {
        var schedules = await _reportRepository.GetAllSchedulesAsync();
        return schedules.Select(MapToDto);
    }

    public async Task RunScheduledReportAsync(ReportSchedule schedule)
    {
        var metrics = await BuildMetricsAsync(schedule.Scope);

        var report = new Report
        {
            Scope = schedule.Scope,
            Metrics = JsonSerializer.Serialize(metrics),
            GeneratedDate = NowIst(),
            ScheduleID = schedule.ScheduleID
        };

        await _reportRepository.SaveReportAsync(report);

        await _notificationService.NotifyReportGeneratedAsync(
            schedule.CreatedBy,
            report.ReportID,
            schedule.Scope.ToString());

        var cron = CronExpression.Parse(schedule.CronExpression);
        schedule.LastRun = NowIst();
        schedule.NextRun = cron.GetNextOccurrence(DateTime.UtcNow, IstZone);

        await _reportRepository.UpdateScheduleAsync(schedule);
    }

    private async Task<object> BuildMetricsAsync(ReportScope scope)
    {
        var totalEnrollments = await _context.Enrollments.CountAsync();
        var activeCertifications = await _context.Certifications
            .CountAsync(c => c.Status == "Active");
        var totalCompliance = await _context.ComplianceRecords.CountAsync();
        var compliantCount = await _context.ComplianceRecords
            .CountAsync(c => c.Status == true);
        var complianceRate = totalCompliance > 0
            ? Math.Round((double)compliantCount / totalCompliance * 100, 2)
            : 0;
        var totalSkillGaps = await _context.SkillGaps.CountAsync();

        return new
        {
            Scope = scope.ToString(),
            TotalEnrollments = totalEnrollments,
            ActiveCertifications = activeCertifications,
            ComplianceRate = complianceRate,
            TotalSkillGaps = totalSkillGaps,
            GeneratedAt = NowIst()
        };
    }

    private static ReportScheduleResponseDto MapToDto(ReportSchedule schedule) => new()
    {
        ScheduleID = schedule.ScheduleID,
        Scope = schedule.Scope,
        CronExpression = schedule.CronExpression,
        CreatedAt = schedule.CreatedAt,
        NextRun = schedule.NextRun,
        IsActive = schedule.IsActive
    };
}
