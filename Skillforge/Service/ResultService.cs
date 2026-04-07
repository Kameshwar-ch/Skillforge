using System;
using Skillforge.Dto;
using Skillforge.Domain;
using Skillforge.Data;
using Skillforge.Repository;

namespace Skillforge.Service;

public class ResultService : IResultService
{
    private readonly SkillForgeDB _context;
    private readonly IResultRepository _resultRepository;

    public ResultService(SkillForgeDB context,IResultRepository resultRepository)
    {
        _context = context;
         _resultRepository = resultRepository;
    }

    public async Task SubmitResultAsync(SubmitAssessmentResultDto request, int reviewerId)
    {
        // Read Assessment (reference only)
        var assessment = await _context.Assessments.FindAsync(request.AssessmentID);

        if (assessment == null)
            throw new Exception("Assessment not found");

         // Validate score <= max
        if (request.Score > assessment.MaxScore)
            throw new Exception("Score exceeds maximum allowed");
        // Validate score <0
        if (request.Score <0)
            throw new Exception("Score should not be negative");
        // Compute pass / fail
        var status = request.Score >= 35 ? true : false;

        // Create Result entity
        var result = new Result
        {
            AssessmentID = request.AssessmentID,
            EmployeeID = request.EmployeeID,
            Score = request.Score,
            Status = status
        };

        // Save Result (via repository)
        _resultRepository.SubmitAssessmentResult(result);

        var AuditLog = new AuditLog
        {
            UserID = reviewerId,
            Action = "Submit Assessment Result",
            Resource = "Result",
            Timestamp = DateTime.UtcNow
        };
        _resultRepository.AddAuditLog(AuditLog);

    }
}
