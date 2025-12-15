using Microsoft.EntityFrameworkCore;
using Npgsql;
using FluentValidation;
using PartitionTableFullStack.API.Common;
using PartitionTableFullStack.API.DAL.Repositories;
using PartitionTableFullStack.API.Data;
using PartitionTableFullStack.API.DTOs;
using PartitionTableFullStack.API.Models;
using System.Text.Json;

namespace PartitionTableFullStack.API.BLL.Services;

public interface IBillService
{
    Task<ServiceResponse<BillCreateResponseDto>> CreateBillAsync(BillCreateRequestDto request, long? userId = null);
    Task<ServiceResponse<PaginatedResponseObject<List<BillListItemDto>>>> GetBillsAsync(short financialYear, QueryParameters queryParams);
    Task<ServiceResponse<BillDetailsResponseDto>> GetBillDetailsAsync(long billId);
    Task<ServiceResponse<List<FinancialYearDto>>> GetFinancialYearsAsync();
}

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly ApplicationDbContext _context;

    public BillService(IBillRepository billRepository, ApplicationDbContext context)
    {
        _billRepository = billRepository;
        _context = context;
    }

    public async Task<ServiceResponse<BillCreateResponseDto>> CreateBillAsync(BillCreateRequestDto request, long? userId = null)
    {
        try
        {
            // Build JSON parameters for the stored procedure
            var billData = new
            {
                bill_date = request.BillDate.ToString("yyyy-MM-dd"),
                bill_mode = request.BillMode,
                reference_no = request.ReferenceNo,
                tr_master_id = request.TrMasterId,
                payment_mode = request.PaymentMode,
                demand = request.Demand,
                major_head = request.MajorHead,
                sub_major_head = request.SubMajorHead,
                minor_head = request.MinorHead,
                plan_status = request.PlanStatus,
                scheme_head = request.SchemeHead,
                detail_head = request.DetailHead,
                voted_charged = request.VotedCharged,
                gross_amount = request.GrossAmount,
                net_amount = request.NetAmount,
                bt_amount = request.BtAmount,
                sanction_no = request.SanctionNo,
                sanction_amt = request.SanctionAmt,
                sanction_date = request.SanctionDate?.ToString("yyyy-MM-dd"),
                sanction_by = request.SanctionBy,
                remarks = request.Remarks,
                ddo_code = request.DdoCode,
                treasury_code = request.TreasuryCode,
                status = request.Status,
                is_gst = request.IsGst,
                gst_amount = request.GstAmount
            };

            var btData = request.BtDetails.Select(bt => new
            {
                bt_serial = bt.BtSerial,
                bt_type = bt.BtType,
                amount = bt.Amount,
                ddo_code = bt.DdoCode,
                treasury_code = bt.TreasuryCode,
                status = bt.Status
            }).ToList();

            var gstData = request.GstDetails.Select(gst => new
            {
                cpin_id = gst.CpinId,
                ddo_gstn = gst.DdoGstn,
                ddo_code = gst.DdoCode,
                tr_id = gst.TrId
            }).ToList();

            var ecsData = request.EcsDetails.Select(ecs => new
            {
                payee_name = ecs.PayeeName,
                beneficiary_id = ecs.BeneficiaryId,
                pan_no = ecs.PanNo,
                contact_number = ecs.ContactNumber,
                address = ecs.Address,
                email = ecs.Email,
                ifsc_code = ecs.IfscCode,
                bank_account_number = ecs.BankAccountNumber,
                bank_name = ecs.BankName,
                amount = ecs.Amount,
                is_gst = ecs.IsGst
            }).ToList();

            var allotmentData = request.AllotmentDetails.Select(allot => new
            {
                allotment_id = allot.AllotmentId,
                amount = allot.Amount,
                ddo_code = allot.DdoCode,
                treasury_code = allot.TreasuryCode,
                active_hoa_id = allot.ActiveHoaId,
                allotment_received = allot.AllotmentReceived,
                progressive_expenses = allot.ProgressiveExpenses
            }).ToList();

            var subvoucherData = request.SubvoucherDetails.Select(sv => new
            {
                subvoucher_no = sv.SubvoucherNo,
                subvoucher_date = sv.SubvoucherDate?.ToString("yyyy-MM-dd"),
                subvoucher_amount = sv.SubvoucherAmount,
                description = sv.Description
            }).ToList();

            // Call stored procedure
            var billDataJson = JsonSerializer.Serialize(billData);
            var btDataJson = JsonSerializer.Serialize(btData);
            var gstDataJson = JsonSerializer.Serialize(gstData);
            var ecsDataJson = JsonSerializer.Serialize(ecsData);
            var allotmentDataJson = JsonSerializer.Serialize(allotmentData);
            var subvoucherDataJson = JsonSerializer.Serialize(subvoucherData);

            var sql = @"SELECT billing.insert_bill_with_components(
                @p_bill_data::jsonb,
                @p_bt_data::jsonb,
                @p_gst_data::jsonb,
                @p_ecs_data::jsonb,
                @p_allotment_data::jsonb,
                @p_subvoucher_data::jsonb,
                @p_user_id
            ) AS ""Value""";

            var parameters = new[]
            {
                new NpgsqlParameter("@p_bill_data", billDataJson),
                new NpgsqlParameter("@p_bt_data", btDataJson),
                new NpgsqlParameter("@p_gst_data", gstDataJson),
                new NpgsqlParameter("@p_ecs_data", ecsDataJson),
                new NpgsqlParameter("@p_allotment_data", allotmentDataJson),
                new NpgsqlParameter("@p_subvoucher_data", subvoucherDataJson),
                new NpgsqlParameter("@p_user_id", userId ?? (object)DBNull.Value)
            };

            var result = await _context.Database.SqlQueryRaw<string>(sql, parameters).FirstOrDefaultAsync();
            
            Console.WriteLine($"Stored procedure result: {result}");
            
            if (result != null)
            {
                var resultObj = JsonSerializer.Deserialize<BillCreateResponseDto>(result);
                
                Console.WriteLine($"Deserialized result - Success: {resultObj?.Success}, BillId: {resultObj?.BillId}, BillNo: {resultObj?.BillNo}, Error: {resultObj?.Error}");
                
                // Check if the stored procedure returned an error
                if (resultObj?.Success == false)
                {
                    var errorMessage = resultObj.Error ?? resultObj.Message ?? "Bill creation failed in stored procedure";
                    
                    return new ServiceResponse<BillCreateResponseDto>
                    {
                        Result = resultObj,
                        ApiResponseStatus = APIResponseStatus.Error,
                        Message = errorMessage
                    };
                }
                
                return new ServiceResponse<BillCreateResponseDto>
                {
                    Result = resultObj,
                    ApiResponseStatus = APIResponseStatus.Success,
                    Message = resultObj?.Message ?? "Bill created successfully"
                };
            }

            return new ServiceResponse<BillCreateResponseDto>
            {
                ApiResponseStatus = APIResponseStatus.Error,
                Message = "No result from stored procedure"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CreateBillAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            
            return new ServiceResponse<BillCreateResponseDto>
            {
                ApiResponseStatus = APIResponseStatus.Error,
                Message = ex.InnerException?.Message ?? ex.Message
            };
        }
    }

    public async Task<ServiceResponse<PaginatedResponseObject<List<BillListItemDto>>>> GetBillsAsync(short financialYear, QueryParameters queryParams)
    {
        try
        {
            // Add financial year filter
            queryParams.Filters.Add(new FilterCriteria
            {
                Field = nameof(BillDetail.FinancialYear),
                Operator = "eq",
                Value = financialYear.ToString()
            });

            // Add is_deleted filter
            queryParams.Filters.Add(new FilterCriteria
            {
                Field = nameof(BillDetail.IsDeleted),
                Operator = "eq",
                Value = "false"
            });

            // Get paginated results
            var paginatedResult = await _billRepository.GetPagedAsync(queryParams);

            // Map using BillMapper
            var dtos = paginatedResult.Data?.Select(BillMapper.ToListItemDto).ToList();

            return new ServiceResponse<PaginatedResponseObject<List<BillListItemDto>>>
            {
                Result = new PaginatedResponseObject<List<BillListItemDto>>
                {
                    Data = dtos,
                    TotalCount = paginatedResult.TotalCount,
                    PageNumber = paginatedResult.PageNumber,
                    PageSize = paginatedResult.PageSize
                },
                ApiResponseStatus = APIResponseStatus.Success,
                Message = "Bills retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<PaginatedResponseObject<List<BillListItemDto>>>
            {
                ApiResponseStatus = APIResponseStatus.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<ServiceResponse<BillDetailsResponseDto>> GetBillDetailsAsync(long billId)
    {
        try
        {
            var bill = await _billRepository.GetBillWithComponentsAsync(billId);

            if (bill == null)
            {
                return new ServiceResponse<BillDetailsResponseDto>
                {
                    ApiResponseStatus = APIResponseStatus.NotFound,
                    Message = "Bill not found"
                };
            }

            // Use BillMapper
            var dto = BillMapper.ToDetailsDto(bill);

            return new ServiceResponse<BillDetailsResponseDto>
            {
                Result = dto,
                ApiResponseStatus = APIResponseStatus.Success,
                Message = "Bill details retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<BillDetailsResponseDto>
            {
                ApiResponseStatus = APIResponseStatus.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<ServiceResponse<List<FinancialYearDto>>> GetFinancialYearsAsync()
    {
        try
        {
            var fys = await _context.FinancialYears
                .OrderByDescending(fy => fy.Id)
                .ToListAsync();

            // Map to DTO - only send required fields
            var dtos = fys.Select(BillMapper.ToFinancialYearDto).ToList();

            return new ServiceResponse<List<FinancialYearDto>>
            {
                Result = dtos,
                ApiResponseStatus = APIResponseStatus.Success,
                Message = "Financial years retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<FinancialYearDto>>
            {
                ApiResponseStatus = APIResponseStatus.Error,
                Message = ex.Message
            };
        }
    }
}
