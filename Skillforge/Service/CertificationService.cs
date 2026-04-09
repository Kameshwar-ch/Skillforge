using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

/// <summary>
/// Orchestrates certification issuance: validates prerequisites,
/// persists the certification, and triggers a notification.
/// </summary>
public class CertificationService : ICertificationService
{
    private readonly ICertificationRepository _certificationRepository;
    private readonly INotificationService _notificationService;

    public CertificationService(
        ICertificationRepository certificationRepository,
        INotificationService notificationService)
    {
        _certificationRepository = certificationRepository;
        _notificationService = notificationService;
    }

    public async Task<(bool Success, string ErrorMessage, CertificationResponseDto? Result)> IssueCertificationAsync(
        IssueCertificationRequestDto dto)
    {
        var employee = await _certificationRepository.GetUserByIdAsync(dto.EmployeeId);
        if (employee == null)
            return (false, "Employee not found.", null);

        var course = await _certificationRepository.GetCourseByIdAsync(dto.CourseId);
        if (course == null)
            return (false, "Course not found.", null);

        if (!course.Status)
            return (false, "Course is not live.", null);

        bool hasPassed = await _certificationRepository.HasPassedAssessmentForCourseAsync(dto.EmployeeId, dto.CourseId);
        if (!hasPassed)
            return (false, "Employee has not passed an assessment for this course.", null);

        bool alreadyCertified = await _certificationRepository.ActiveCertificationExistsAsync(dto.EmployeeId, dto.CourseId);
        if (alreadyCertified)
            return (false, "An active certification already exists for this employee and course.", null);

        var issueDate = DateTime.Now;
        var certification = new Certification
        {
            EmployeeID = dto.EmployeeId,
            CourseID = dto.CourseId,
            IssueDate = issueDate,
            ExpiryDate = issueDate.AddYears(1),
            Status = "Active"
        };

        int certificationId = await _certificationRepository.IssueCertificationAsync(certification);

        await _notificationService.NotifyCertificationIssuedAsync(dto.EmployeeId, certificationId);

        return (true, null!, new CertificationResponseDto
        {
            CertificationId = certificationId,
            EmployeeId = dto.EmployeeId,
            CourseId = dto.CourseId,
            IssueDate = certification.IssueDate,
            ExpiryDate = certification.ExpiryDate,
            Status = certification.Status
        });
    }
}
