using System;
using Skillforge.Dto;

namespace Skillforge.Service;

public interface IResultService
{
    Task SubmitResultAsync(SubmitAssessmentResultDto request, int reviewerId);
}
