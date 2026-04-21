using Skillforge.Domain;
using Skillforge.Dto;

namespace Skillforge.Service;

public interface IReportService
{
    Task<ReportScheduleResponseDto> CreateScheduleAsync(CreateReportScheduleDto dto, int adminId);
    Task<IEnumerable<ReportScheduleResponseDto>> GetAllSchedulesAsync();
    Task RunScheduledReportAsync(ReportSchedule schedule);
}
