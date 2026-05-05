using System;

namespace Skillforge.Service;

using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using Skillforge.Dto;
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

    // Bulk enrollment: Manager assigns a course to multiple employees at once
    // Supports partial success - valid employees are enrolled, invalid ones are skipped with reason
    // Audits the entire bulk operation as a single ManagerAssign action
    public async Task<BulkEnrollmentResponseDto> BulkEnrollAsync(BulkEnrollmentRequestDto request, int managerId)
    {
        var response = new BulkEnrollmentResponseDto
        {
            TotalRequested = request.EmployeeIds.Count
        };

        // 1. Validate course exists
        Course course = await enrollmentRepository.GetByIdAsync(request.CourseId);

        if (course == null)
        {
            throw new KeyNotFoundException(EnrollmentMessages.notfound);
        }

        // 2. Validate course is open for enrollment
        if (course.Status == false)
        {
            throw new BadHttpRequestException(EnrollmentMessages.closed);
        }

        // 3. Process each employee - partial success: skip failures, continue with valid ones
        foreach (var employeeId in request.EmployeeIds)
        {
            var resultItem = new EnrollmentResultItem { EmployeeId = employeeId };

            // Check if employee exists and is active
            var employeeExists = await enrollmentRepository.EmployeeExistsAsync(employeeId);
            if (!employeeExists)
            {
                resultItem.Status = "Failed";
                resultItem.Reason = EnrollmentMessages.EmployeeNotFound;
                response.Failed++;
                response.Results.Add(resultItem);
                continue;
            }

            // Check for duplicate enrollment
            var alreadyEnrolled = await enrollmentRepository.ExistsAsync(request.CourseId, employeeId);
            if (alreadyEnrolled)
            {
                resultItem.Status = "Failed";
                resultItem.Reason = EnrollmentMessages.enrolled;
                response.Failed++;
                response.Results.Add(resultItem);
                continue;
            }

            // Create enrollment for valid employee
            var enrollment = new Enrollment
            {
                CourseID = request.CourseId,
                EmployeeID = employeeId,
                EnrollmentDate = DateTime.UtcNow
            };
            await enrollmentRepository.AddAsync(enrollment);

            resultItem.EnrollmentId = enrollment.EnrollmentID;
            resultItem.Status = "Success";
            response.Succeeded++;
            response.Results.Add(resultItem);
        }

        // 4. Audit: Log the ManagerAssign action with bulk operation summary
        await auditService.LogAsync(
            managerId,
            $"ManagerAssign - BulkEnroll CourseId:{request.CourseId} Total:{response.TotalRequested} Succeeded:{response.Succeeded} Failed:{response.Failed}",
            "Enrollment"
        );

        return response;
    }
}
