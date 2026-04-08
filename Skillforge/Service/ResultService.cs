using System;
using Skillforge.Dto;
using Skillforge.Domain;
using Skillforge.Data;
using Skillforge.Repository;
using System.Net.Mail;
using Skillforge.Utility;
using Microsoft.EntityFrameworkCore;

namespace Skillforge.Service;

public class ResultService : IResultService
{
    private readonly SkillForgeDB _context;
    private readonly IResultRepository _resultRepository;
    private readonly IConfiguration _configuration;

    public ResultService(SkillForgeDB context,IResultRepository resultRepository,IConfiguration configuration)
    {
        _context = context;
         _resultRepository = resultRepository;
         _configuration = configuration;
    }

    public async Task SubmitResultAsync(SubmitAssessmentResultDto request, int reviewerId)
    {
        
        bool exists = await _context.Results.AnyAsync(r => r.AssessmentID == request.AssessmentID 
                && r.EmployeeID == request.EmployeeID);

        if (exists)
            throw new Exception(ResultMessages.Duplicate);

        // Read Assessment (reference only)
        var assessment = await _context.Assessments.FindAsync(request.AssessmentID);

        if (assessment == null)
            throw new KeyNotFoundException(ResultMessages.NotFound);

         // Validate score <= max
        if (request.Score > assessment.MaxScore)
            throw new Exception(ResultMessages.exceeds);
        // Validate score <0
        if (request.Score <0)
            throw new Exception(ResultMessages.negative);
        // Compute pass / fail
        var passingScore = _configuration.GetValue<int>("AssessmentSettings:PassingScore");
        var status = request.Score >= passingScore ? ResultStatus.Pass :ResultStatus.Fail;

        // Create Result entity
        var result = new Result
        {
            AssessmentID = request.AssessmentID,
            EmployeeID = request.EmployeeID,
            Score = request.Score,
            Status = status
        };

        // Save Result (via repository)
        await _resultRepository.SubmitAssessmentResult(result);

        var AuditLog = new AuditLog
        {
            UserID = reviewerId,
            Action = "Submit Assessment Result",
            Resource = "Result",
            Timestamp = DateTime.UtcNow
        };
        await _resultRepository.AddAuditLog(AuditLog);

    }
}
