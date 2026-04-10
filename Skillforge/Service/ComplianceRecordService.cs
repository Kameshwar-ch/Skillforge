using System;
using Skillforge.Domain;
using Skillforge.Dto.ComplianceRecordDto;
using Skillforge.Domain;
using Skillforge.Repository;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Utility;

namespace Skillforge.Service;

public class ComplianceRecordService : IComplianceRecordService
{
    private readonly IComplianceRecord _complianceRecordRepo;
    private readonly ICertificationRepository _CertificationRepository;
    public ComplianceRecordService(IComplianceRecord ComplianceRecordRepository, ICertificationRepository CertificationRepository)
    {
        _complianceRecordRepo = ComplianceRecordRepository;
        _CertificationRepository = CertificationRepository;
    }
    public async Task<ComplianceSummaryDto> GetComplianceSummaryAsync()
    {
        IEnumerable<ComplianceRecord> crs = await _complianceRecordRepo.GetComplianceRecordAsync();
        List<GetComplianceDto> ComplianceRecordDtos = crs.Select(c => new GetComplianceDto(c.ComplianceID, c.EmployeeID, c.Employee.Name, c.CertificationID, c.Certification.Course.Title, c.Status, c.Date)).ToList();
        int TotalEmp = crs.Select(c => c.EmployeeID).Distinct().Count();
        int CompliantEmp = crs.GroupBy(c => c.EmployeeID).Count(c => c.All(c => c.Status));
        int NonCompliantEmp = TotalEmp - CompliantEmp;
        double CompliantPercent = (CompliantEmp / TotalEmp) * 100;
        ComplianceSummaryDto csd = new ComplianceSummaryDto(TotalEmp, CompliantEmp, NonCompliantEmp, CompliantPercent, ComplianceRecordDtos);
        return csd;
    }

    public async Task<string> UpdateComplianceRecords()
    {
        await _complianceRecordRepo.DeleteComplianceRecords();
        List<Certification> certifications = await _CertificationRepository.GetAllCertifications();
        List<ComplianceRecord> complianceRecords = new List<ComplianceRecord>();
        DateTime today = DateTime.UtcNow;
        foreach (Certification certificate in certifications)
        {
            ComplianceRecord cr = new ComplianceRecord();
            cr.CertificationID = certificate.CertificationID;
            cr.EmployeeID = certificate.EmployeeID;
            cr.Date = today;
            if (certificate.ExpiryDate >= DateTime.Now)
                cr.Status = true;
            else
                cr.Status = false;
            complianceRecords.Add(cr);
        }
        await _complianceRecordRepo.PostComplianceRecords(complianceRecords);
        return ComplianceRecordUtility.UpdateComplianceRecords;
    }

}
