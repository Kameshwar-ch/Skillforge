using System;
using System.Collections;
using Skillforge.Domain;
using Skillforge.Dto.ComplianceRecordDto;

namespace Skillforge.Repository;

public interface IComplianceRecord
{
    public Task<IEnumerable<ComplianceRecord>> GetComplianceRecordAsync();
    public Task PostComplianceRecords(IEnumerable<ComplianceRecord> complianceRecords);
    public Task DeleteComplianceRecords();
    public Task AddComplianceRecord(ComplianceRecord cr);
}
