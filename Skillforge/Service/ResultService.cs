using System;
using Skillforge.Dto;
using Skillforge.Domain;
using Skillforge.Data;
using Skillforge.Repository;
using System.Net.Mail;
using Skillforge.Utility;

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
            throw new Exception(ResultMessages.NotFound);

         // Validate score <= max
        if (request.Score > assessment.MaxScore)
            throw new Exception(ResultMessages.exceeds);
        // Validate score <0
        if (request.Score <0)
            throw new Exception(ResultMessages.negative);
        // Compute pass / fail
        var status = request.Score >= 35 ? ResultStatus.Pass :ResultStatus.Fail;

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
