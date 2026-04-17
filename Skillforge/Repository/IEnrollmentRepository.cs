using System;
using Skillforge.Domain;

namespace Skillforge.Repository;

public interface IEnrollmentRepository
{
    Task<Course> GetByIdAsync(int courseId);
    Task AddAuditLog(AuditLog auditLog);
    Task<bool> ExistsAsync(int courseId, int employeeId);
    Task AddAsync(Enrollment enrollment);
}
