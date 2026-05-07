using System;
using Skillforge.Dto;

namespace Skillforge.Service;

public interface IEnrollmentService
{
    Task<long> EnrollAsync(int courseId, int employeeId);
    Task<BulkEnrollmentResponseDto> BulkEnrollAsync(BulkEnrollmentRequestDto request, int managerId);
}
