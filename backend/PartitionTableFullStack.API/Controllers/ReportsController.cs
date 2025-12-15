using Microsoft.AspNetCore.Mvc;
using PartitionTableFullStack.API.BLL.Services;

namespace PartitionTableFullStack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Download bill report as Excel for specified financial year
    /// </summary>
    [HttpGet("excel")]
    public async Task<IActionResult> DownloadExcelReport([FromQuery] short financialYear)
    {
        var excelBytes = await _reportService.GenerateExcelReportAsync(financialYear);
        
        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"BillReport_FY{financialYear}_{DateTime.Now:yyyyMMdd}.xlsx"
        );
    }

    /// <summary>
    /// Download bill report as PDF for specified financial year
    /// </summary>
    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadPdfReport([FromQuery] short financialYear)
    {
        var pdfBytes = await _reportService.GeneratePdfReportAsync(financialYear);
        
        return File(
            pdfBytes,
            "application/pdf",
            $"BillReport_FY{financialYear}_{DateTime.Now:yyyyMMdd}.pdf"
        );
    }
}
