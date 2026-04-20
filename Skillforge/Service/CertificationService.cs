using Skillforge.Constants;
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
            return (false, CertificationErrorMessages.EmployeeNotFound, null);

        var course = await _certificationRepository.GetCourseByIdAsync(dto.CourseId);
        if (course == null)
            return (false, CertificationErrorMessages.CourseNotFound, null);

        if (!course.Status)
            return (false, CertificationErrorMessages.CourseNotLive, null);

        bool hasPassed = await _certificationRepository.HasPassedAssessmentForCourseAsync(dto.EmployeeId, dto.CourseId);
        if (!hasPassed)
            return (false, CertificationErrorMessages.AssessmentNotPassed, null);

        var existingCertification = await _certificationRepository.GetActiveCertificationAsync(dto.EmployeeId, dto.CourseId);
        if (existingCertification != null)
            return (false, CertificationErrorMessages.ActiveCertificationExists, new CertificationResponseDto
            {
                CertificationId = existingCertification.CertificationID,
                EmployeeId = existingCertification.EmployeeID,
                CourseId = existingCertification.CourseID,
                CourseName = course.Title,
                CourseDescription = course.Description,
                IssueDate = existingCertification.IssueDate,
                ExpiryDate = existingCertification.ExpiryDate,
                Status = existingCertification.Status
            });

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
            CourseName = course.Title,
            CourseDescription = course.Description,
            IssueDate = certification.IssueDate,
            ExpiryDate = certification.ExpiryDate,
            Status = certification.Status
        });
    }
}
