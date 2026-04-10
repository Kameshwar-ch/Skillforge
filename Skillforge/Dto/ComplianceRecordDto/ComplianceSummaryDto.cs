using System;

namespace Skillforge.Dto.ComplianceRecordDto;

public record ComplianceSummaryDto(
    int TotalEmployees,
    int CompliantCount,
    int NonCompliantCount,
    double ComplianceRate,
    List<GetComplianceDto> Records
);