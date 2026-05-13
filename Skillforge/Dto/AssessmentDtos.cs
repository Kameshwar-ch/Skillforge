using System.ComponentModel.DataAnnotations;
using Skillforge.Domain;

namespace Skillforge.Dto;

/// <summary>
/// Request DTO for creating a new assessment.
/// Contains the target course, assessment type, and maximum achievable score.
/// </summary>
public class CreateAssessmentRequestDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public AssessmentType Type { get; set; }

    /// <summary>
    /// The maximum score achievable in this assessment. Must be between 1 and 100.
    /// </summary>
    [Required]
    [Range(1, 100, ErrorMessage = "MaxScore must be between 1 and 100.")]
    public decimal MaxScore { get; set; }
}

/// <summary>
/// Response DTO returned after successfully creating an assessment.
/// Contains the auto-generated identifier for the new assessment.
/// </summary>
public class CreateAssessmentResponseDto
{
    public int AssessmentId { get; set; }
}

/// <summary>
/// Response DTO returned after successfully updating an assessment.
/// </summary>
public class UpdateAssessmentRequestDto
{
    [Required]
    public AssessmentType Type { get; set; }
    [Required]
    [Range(1, 100, ErrorMessage = "MaxScore must be between 1 and 100.")]
    public decimal MaxScore { get; set; }
}

public class AssessmentFilterDto
{
    public int? CourseId { get; set; }
    public AssessmentType? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class AssessmentListDto
{
    public int AssessmentId { get; set; }
    public int CourseId { get; set; }
    public AssessmentType Type { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime Date { get; set; }
}

