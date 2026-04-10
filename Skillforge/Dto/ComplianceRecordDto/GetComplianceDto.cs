using System;

namespace Skillforge.Dto.ComplianceRecordDto;

public record GetComplianceDto(
int ComplianceId,    
int EmployeeId,
string EmployeeName,
int CertificationId,
string CourseName,
bool Status,
DateTime Date
);
