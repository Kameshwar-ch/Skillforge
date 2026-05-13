using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

/// <summary>
/// Implements the business logic for assessment management.
/// Validates course existence and live status before persisting a new assessment via the repository.
/// </summary>
public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _assessmentRepository;

    public AssessmentService(IAssessmentRepository assessmentRepository)
    {
        _assessmentRepository = assessmentRepository;
    }

    /// <summary>
    /// Validates the target course and creates a new assessment if all rules pass.
    /// Returns a failure result if the course does not exist or is not currently live.
    /// </summary>
    /// <param name="dto">The assessment creation request containing CourseId, Type, and MaxScore.</param>
    /// <returns>
    /// A tuple with Success set to true and the new AssessmentId on success,
    /// or Success set to false with an ErrorMessage describing the validation failure.
    /// </returns>
    public async Task<(bool Success, string ErrorMessage, int AssessmentId)> CreateAssessmentAsync(CreateAssessmentRequestDto dto)
    {
        var course = await _assessmentRepository.GetCourseByIdAsync(dto.CourseId);

        if (course == null)
            return (false, "Course not found.", 0);

        if (!course.Status)
            return (false, "Course is not live.", 0);

        var assessment = new Assessment
        {
            CourseID = dto.CourseId,
            Type = dto.Type,
            MaxScore = dto.MaxScore,
            Date = DateTime.Now
        };

        int assessmentId = await _assessmentRepository.CreateAssessmentAsync(assessment);
        return (true, null!, assessmentId);
    }

    public async Task<(bool Success, string ErrorMessage)> UpdateAssessmentAsync(int assessmentId, UpdateAssessmentRequestDto dto)
    {
        var existingAssessment = await _assessmentRepository.GetAssessmentByIdAsync(assessmentId);

        if (existingAssessment == null)
            return (false, "Assessment not found.");

        existingAssessment.Type = dto.Type;
        existingAssessment.MaxScore = dto.MaxScore;

        await _assessmentRepository.UpdateAssessmentAsync(existingAssessment);

        return (true, null!);
    }

    public async Task<(bool Success, string ErrorMessage)> DeleteAssessmentAsync(int assessmentId)
    {
        var assessment = await _assessmentRepository.GetAssessmentByIdAsync(assessmentId);

        if (assessment == null)
            return (false, "Assessment not found.");

        await _assessmentRepository.DeleteAssessmentAsync(assessment);

        return (true, null!);
    }

    public async Task<List<AssessmentListDto>> GetAssessmentsAsync(AssessmentFilterDto filter)
    {
        var assessments = await _assessmentRepository.GetAssessmentsAsync(filter);

        return assessments.Select(a => new AssessmentListDto
        {
        AssessmentId = a.AssessmentID,
        CourseId = a.CourseID,
        Type = a.Type,
        MaxScore = a.MaxScore,
        Date = a.Date
        }).ToList();
    }

    public async Task<AssessmentListDto?> GetAssessmentByIdAsync(int assessmentId)
    {
        var assessment = await _assessmentRepository.GetAssessmentByIdAsync(assessmentId);

        if (assessment == null) return null;

        return new AssessmentListDto
        {
        AssessmentId = assessment.AssessmentID,
        CourseId = assessment.CourseID,
        Type = assessment.Type,
        MaxScore = assessment.MaxScore,
        Date = assessment.Date
        };
    }

}
