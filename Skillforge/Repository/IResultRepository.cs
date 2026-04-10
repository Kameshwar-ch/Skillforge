using System;
using Skillforge.Domain;
namespace Skillforge.Repository;

public interface IResultRepository
{
    Task SubmitAssessmentResult(Result result);
    Task AddAuditLog(AuditLog auditLog);
}
