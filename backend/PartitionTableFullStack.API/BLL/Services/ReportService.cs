using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Data;
using PartitionTableFullStack.API.DTOs;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PartitionTableFullStack.API.BLL.Services;

public class ReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
        
        // Set EPPlus license context (NonCommercial for demo)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        // Set QuestPDF license (Community for demo)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<BillReportSummaryDto> GetReportDataAsync(short financialYear)
    {
        var bills = await _context.BillDetails
            .Where(b => b.FinancialYear == financialYear && !b.IsDeleted)
            .OrderBy(b => b.BillDate)
            .Select(b => new BillReportItemDto
            {
                BillNo = b.BillNo,
                BillDate = b.BillDate,
                DdoCode = b.DdoCode,
                GrossAmount = b.GrossAmount,
                NetAmount = b.NetAmount,
                BtAmount = b.BtAmount,
                GstAmount = b.GstAmount,
                Status = b.Status
            })
            .ToListAsync();

        var fyInfo = await _context.FinancialYears.FirstOrDefaultAsync(fy => fy.Id == financialYear);

        return new BillReportSummaryDto
        {
            FinancialYear = financialYear,
            FinancialYearText = fyInfo?.FinancialYear ?? $"FY {financialYear}",
            TotalBills = bills.Count,
            TotalGrossAmount = bills.Sum(b => b.GrossAmount ?? 0),
            TotalNetAmount = bills.Sum(b => b.NetAmount ?? 0),
            TotalBtAmount = bills.Sum(b => b.BtAmount ?? 0),
            TotalGstAmount = bills.Sum(b => b.GstAmount ?? 0),
            Bills = bills
        };
    }

    public async Task<byte[]> GenerateExcelReportAsync(short financialYear)
    {
        var reportData = await GetReportDataAsync(financialYear);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Bill Report");

        // Header
        worksheet.Cells["A1"].Value = $"Bill Report - {reportData.FinancialYearText}";
        worksheet.Cells["A1:G1"].Merge = true;
        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.Font.Size = 14;
        worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        // Summary
        worksheet.Cells["A2"].Value = "Summary";
        worksheet.Cells["A2"].Style.Font.Bold = true;
        worksheet.Cells["A3"].Value = "Total Bills:";
        worksheet.Cells["B3"].Value = reportData.TotalBills;
        worksheet.Cells["A4"].Value = "Total Gross Amount:";
        worksheet.Cells["B4"].Value = reportData.TotalGrossAmount;
        worksheet.Cells["A5"].Value = "Total Net Amount:";
        worksheet.Cells["B5"].Value = reportData.TotalNetAmount;
        worksheet.Cells["A6"].Value = "Total BT Amount:";
        worksheet.Cells["B6"].Value = reportData.TotalBtAmount;
        worksheet.Cells["A7"].Value = "Total GST Amount:";
        worksheet.Cells["B7"].Value = reportData.TotalGstAmount;

        // Table headers
        int row = 9;
        worksheet.Cells[row, 1].Value = "Bill No";
        worksheet.Cells[row, 2].Value = "Bill Date";
        worksheet.Cells[row, 3].Value = "DDO Code";
        worksheet.Cells[row, 4].Value = "Gross Amount";
        worksheet.Cells[row, 5].Value = "Net Amount";
        worksheet.Cells[row, 6].Value = "BT Amount";
        worksheet.Cells[row, 7].Value = "GST Amount";
        worksheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;

        // Data
        row++;
        foreach (var bill in reportData.Bills)
        {
            worksheet.Cells[row, 1].Value = bill.BillNo;
            worksheet.Cells[row, 2].Value = bill.BillDate.ToString("dd-MMM-yyyy");
            worksheet.Cells[row, 3].Value = bill.DdoCode;
            worksheet.Cells[row, 4].Value = bill.GrossAmount;
            worksheet.Cells[row, 5].Value = bill.NetAmount;
            worksheet.Cells[row, 6].Value = bill.BtAmount;
            worksheet.Cells[row, 7].Value = bill.GstAmount;
            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    public async Task<byte[]> GeneratePdfReportAsync(short financialYear)
    {
        var reportData = await GetReportDataAsync(financialYear);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4.Landscape());

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().AlignCenter().Text($"Bill Report - {reportData.FinancialYearText}")
                            .FontSize(18).Bold();
                        column.Item().AlignCenter().Text($"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm}")
                            .FontSize(10);
                    });
                });

                page.Content().PaddingVertical(10).Column(column =>
                {
                    // Summary section
                    column.Item().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Summary").FontSize(14).Bold();
                            col.Item().Text($"Total Bills: {reportData.TotalBills}");
                            col.Item().Text($"Total Gross Amount: {reportData.TotalGrossAmount:N0}");
                            col.Item().Text($"Total Net Amount: {reportData.TotalNetAmount:N0}");
                            col.Item().Text($"Total BT Amount: {reportData.TotalBtAmount:N0}");
                            col.Item().Text($"Total GST Amount: {reportData.TotalGstAmount:N0}");
                        });
                    });

                    // Table
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        // Headers
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Bill No").Bold();
                            header.Cell().Element(CellStyle).Text("Bill Date").Bold();
                            header.Cell().Element(CellStyle).Text("DDO Code").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Gross Amount").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Net Amount").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("BT Amount").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("GST Amount").Bold();
                        });

                        // Rows
                        foreach (var bill in reportData.Bills)
                        {
                            table.Cell().Element(CellStyle).Text(bill.BillNo);
                            table.Cell().Element(CellStyle).Text(bill.BillDate.ToString("dd-MMM-yyyy"));
                            table.Cell().Element(CellStyle).Text(bill.DdoCode);
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.GrossAmount:N0}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.NetAmount:N0}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.BtAmount:N0}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.GstAmount:N0}");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
    }
}
