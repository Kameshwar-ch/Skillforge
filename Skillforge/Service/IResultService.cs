using System;
using Skillforge.Dto;

namespace Skillforge.Service;

public interface IResultService
{
    Task SubmitResultAsync(SubmitAssessmentResultDto request, int reviewerId);
    Task<List<ResultViewDto>> GetResultsByAssessmentAsync(int assessmentId);
    Task UpdateResultAsync(int assessmentId, int employeeId, UpdateResultDto dto, int reviewerId);
    Task DeleteResultAsync(int assessmentId, int employeeId, int reviewerId);
}
