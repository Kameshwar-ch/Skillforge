using System;
using Skillforge.Domain;

namespace Skillforge.Dto;

public class SubmitAssessmentResultDto
{
    public int AssessmentID { get; set; }
    public int EmployeeID { get; set; }
    public decimal Score { get; set; }
}

public class ResultViewDto
{
    public int EmployeeID { get; set; }
    public decimal Score { get; set; }
    public ResultStatus Status { get; set; }
}

public class UpdateResultDto
{
    public decimal Score { get; set; }
}