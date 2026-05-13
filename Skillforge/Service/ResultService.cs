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
            Timestamp = DateTime.Now
        };
        await _resultRepository.AddAuditLog(AuditLog);

    }

    public async Task<List<ResultViewDto>> GetResultsByAssessmentAsync(int assessmentId)
    {
        var results = await _context.Results
        .Where(r => r.AssessmentID == assessmentId)
        .ToListAsync();

        return results.Select(r => new ResultViewDto
        {
        EmployeeID = r.EmployeeID,
        Score = r.Score,
        Status = r.Status
        }).ToList();
    }

    public async Task UpdateResultAsync(int assessmentId, int employeeId, UpdateResultDto dto, int reviewerId)
    {
        var result = await _context.Results
            .FirstOrDefaultAsync(r => r.AssessmentID == assessmentId && r.EmployeeID == employeeId);

        if (result == null)
            throw new KeyNotFoundException("Result not found");

        var assessment = await _context.Assessments.FindAsync(assessmentId);

        if (assessment == null)
            throw new KeyNotFoundException("Assessment not found");

        if (dto.Score > assessment.MaxScore)
            throw new Exception(ResultMessages.exceeds);

        if (dto.Score < 0)
            throw new Exception(ResultMessages.negative);

        var passingScore = _configuration.GetValue<int>("AssessmentSettings:PassingScore");

        result.Score = dto.Score;
        result.Status = dto.Score >= passingScore ? ResultStatus.Pass : ResultStatus.Fail;

        await _context.SaveChangesAsync();

        var audit = new AuditLog
        {
            UserID = reviewerId,
            Action = "Update Assessment Result",
            Resource = $"Result/{assessmentId}/{employeeId}",
            Timestamp = DateTime.Now
        };

        await _resultRepository.AddAuditLog(audit);
    }

    public async Task DeleteResultAsync(int assessmentId, int employeeId, int reviewerId)
    {
        var result = await _context.Results
        .FirstOrDefaultAsync(r => r.AssessmentID == assessmentId && r.EmployeeID == employeeId);

        if (result == null)
        throw new KeyNotFoundException("Result not found");

        
        _context.Results.Remove(result);
        await _context.SaveChangesAsync();


        // Audit log
        var audit = new AuditLog
        {
        UserID = reviewerId,
        Action = "Delete Assessment Result",
        Resource = $"Result/{assessmentId}/{employeeId}",
        Timestamp = DateTime.Now
        };

        await _resultRepository.AddAuditLog(audit);
    }
}
