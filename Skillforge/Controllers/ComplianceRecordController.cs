using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto.ComplianceRecordDto;
using Skillforge.Service;

namespace Skillforge.Controllers;

[Route("api/[controller]")]
[ApiController]

// this depends on the service layer
public class ComplianceController : ControllerBase
{
    private readonly IComplianceRecordService _ComplianceRecordService;
    public ComplianceController(IComplianceRecordService ComplianceRecordService)
    {
        _ComplianceRecordService = ComplianceRecordService;
    }

    [HttpGet("Summary")]
    public async Task<ActionResult<ComplianceSummaryDto>> GetComplianceSummary()
    {
        ComplianceSummaryDto csd = await _ComplianceRecordService.GetComplianceSummaryAsync();
        if (csd == null)
            return NotFound();
        return Ok(csd);
    }

    [HttpGet("Refresh")]
    public async Task<ActionResult<string>> RefreshComplianceRecords()
    {
        return Ok(await _ComplianceRecordService.UpdateComplianceRecords());
    }
}
