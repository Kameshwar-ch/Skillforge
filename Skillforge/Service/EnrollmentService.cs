using System;

namespace Skillforge.Service;

using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using Skillforge.Repository;
using Skillforge.Utility;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository enrollmentRepository;
    private readonly IAuditService auditService;

    public EnrollmentService(IEnrollmentRepository _enrollmentRepository,IAuditService _auditService)
    {
        enrollmentRepository = _enrollmentRepository;
        auditService = _auditService;
    }

    public async Task<long> EnrollAsync(int courseId, int employeeId)
    {
        // 1. Get course
        Course course = await enrollmentRepository.GetByIdAsync(courseId);

        if (course == null)
        {
            throw new KeyNotFoundException(EnrollmentMessages.notfound);
        }

        // 2. Course closed → 400
        if (course.Status == false)
        {
            var auditLog = new AuditLog
            {
            UserID = employeeId,
            Action = $"FAILED- {courseId} COURSE CLOSED",
            Resource = "Enrollment",
            Timestamp = DateTime.UtcNow
            };
            await enrollmentRepository.AddAuditLog(auditLog);
            throw new BadHttpRequestException(EnrollmentMessages.closed);
        }

        // 3. Duplicate enroll → 409
        var exists = await enrollmentRepository.ExistsAsync(courseId, employeeId);

        if (exists)
        {
            var auditLog = new AuditLog
            {
            UserID = employeeId,
            Action = $"FAILED- {courseId} DUPLICATE",
            Resource = "Enrollment",
            Timestamp = DateTime.UtcNow
            };
            await enrollmentRepository.AddAuditLog(auditLog);
            throw new InvalidOperationException(EnrollmentMessages.enrolled);
        }

        // 4. Create enrollment
        var enrollment = new Enrollment
        {
            CourseID = courseId,
            EmployeeID = employeeId,
            EnrollmentDate = DateTime.UtcNow
        };
        await enrollmentRepository.AddAsync(enrollment);

        // 5. Audit success
        var AuditLog = new AuditLog
        {
            UserID = employeeId,
            Action = $"SUCCESS - {courseId} Enrolled",
            Resource = "Enrollment",
            Timestamp = DateTime.UtcNow
        };
        await enrollmentRepository.AddAuditLog(AuditLog);

        return enrollment.EnrollmentID;
    }
}
