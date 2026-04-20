using Skillforge.Dto;

namespace Skillforge.Service;

public interface IAttendanceService
{
    Task<AttendanceResponseDto>          MarkAttendanceAsync(MarkAttendanceDto dto, int trainerID);
    Task<BulkAttendanceResponseDto>      BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto, int trainerID);
    Task<GetCourseAttendanceResponseDto> GetCourseAttendanceAsync(int courseID, DateTime date, int trainerID);
}