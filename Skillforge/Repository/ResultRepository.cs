using System;
using Skillforge.Domain;
using Skillforge.Data;
namespace Skillforge.Repository;

public class ResultRepository : IResultRepository
{
    private readonly SkillForgeDB context;

    public ResultRepository(SkillForgeDB context)
    {
        this.context = context;
    }

    public async Task AddAuditLog(AuditLog auditLog)
    {
        await context.AuditLogs.AddAsync(auditLog);
        await context.SaveChangesAsync();
    }

    public async Task SubmitAssessmentResult(Result result)
    {
        await context.Results.AddAsync(result);
        await context.SaveChangesAsync();
    }
}
