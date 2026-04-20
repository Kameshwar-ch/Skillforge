using System;

namespace Skillforge.Service;

public interface IEnrollmentService
{
    Task<long> EnrollAsync(int courseId, int employeeId);
}
