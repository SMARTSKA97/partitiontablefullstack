using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PartitionTableFullStack.API.BLL.Services;
using PartitionTableFullStack.API.Common;
using PartitionTableFullStack.API.DTOs;

namespace PartitionTableFullStack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    /// <summary>
    /// Create a new bill with all sub-components (BT, GST, ECS, Allotment, Subvoucher)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateBill(
        [FromBody] BillCreateRequestDto request,
        [FromServices] IValidator<BillCreateRequestDto> validator)
    {
        // Validate request using FluentValidation
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var response = new ServiceResponse<BillCreateResponseDto>
            {
                ApiResponseStatus = APIResponseStatus.ValidationError,
                Message = "Validation failed",
                ValidationResults = validationResult.Errors.Select(e => (object)new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList()
            };
            return BadRequest(response);
        }

        var result = await _billService.CreateBillAsync(request, userId: 1);

        return result.ApiResponseStatus switch
        {
            APIResponseStatus.Success => Ok(result),
            APIResponseStatus.ValidationError => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }

    /// <summary>
    /// Get list of bills filtered by financial year with advanced query parameters
    /// </summary>
    [HttpPost("query")]
    public async Task<ActionResult> GetBills(
        [FromQuery] short financialYear,
        [FromBody] QueryParameters queryParams)
    {
        var result = await _billService.GetBillsAsync(financialYear, queryParams);

        return result.ApiResponseStatus switch
        {
            APIResponseStatus.Success => Ok(result),
            _ => StatusCode(500, result)
        };
    }

    /// <summary>
    /// Get detailed information for a specific bill
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetBillDetails(long id)
    {
        var result = await _billService.GetBillDetailsAsync(id);

        return result.ApiResponseStatus switch
        {
            APIResponseStatus.Success => Ok(result),
            APIResponseStatus.NotFound => NotFound(result),
            _ => StatusCode(500, result)
        };
    }

    /// <summary>
    /// Get available financial years
    /// </summary>
    [HttpGet("financial-years")]
    public async Task<ActionResult> GetFinancialYears()
    {
        var result = await _billService.GetFinancialYearsAsync();
        
        return result.ApiResponseStatus switch
        {
            APIResponseStatus.Success => Ok(result),
            _ => StatusCode(500, result)
        };
    }
}
