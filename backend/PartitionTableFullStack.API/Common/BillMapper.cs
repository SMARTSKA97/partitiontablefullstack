using PartitionTableFullStack.API.DTOs;
using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.Common;

public static class BillMapper
{
    public static BillListItemDto ToListItemDto(BillDetail bill)
    {
        return new BillListItemDto
        {
            BillId = bill.BillId,
            BillNo = bill.BillNo,
            BillDate = bill.BillDate,
            DdoCode = bill.DdoCode,
            GrossAmount = bill.GrossAmount,
            NetAmount = bill.NetAmount,
            Status = bill.Status,
            Remarks = bill.Remarks,
            FinancialYear = bill.FinancialYear
        };
    }

    public static BillDetailsResponseDto ToDetailsDto(BillDetail bill)
    {
        return new BillDetailsResponseDto
        {
            BillId = bill.BillId,
            BillNo = bill.BillNo,
            BillDate = bill.BillDate,
            BillMode = bill.BillMode,
            ReferenceNo = bill.ReferenceNo,
            TrMasterId = bill.TrMasterId,
            PaymentMode = bill.PaymentMode,
            FinancialYear = bill.FinancialYear,
            Demand = bill.Demand,
            MajorHead = bill.MajorHead,
            SubMajorHead = bill.SubMajorHead,
            MinorHead = bill.MinorHead,
            PlanStatus = bill.PlanStatus,
            SchemeHead = bill.SchemeHead,
            DetailHead = bill.DetailHead,
            VotedCharged = bill.VotedCharged,
            GrossAmount = bill.GrossAmount,
            NetAmount = bill.NetAmount,
            BtAmount = bill.BtAmount,
            SanctionNo = bill.SanctionNo,
            SanctionAmt = bill.SanctionAmt,
            SanctionDate = bill.SanctionDate,
            SanctionBy = bill.SanctionBy,
            Remarks = bill.Remarks,
            DdoCode = bill.DdoCode,
            TreasuryCode = bill.TreasuryCode,
            Status = bill.Status,
            IsGst = bill.IsGst ?? false,
            GstAmount = bill.GstAmount,
            CreatedAt = bill.CreatedAt,

            BtDetails = bill.BtDetails.Select(bt => new BtDetailDto
            {
                BtSerial = bt.BtSerial,
                BtType = bt.BtType,
                Amount = bt.Amount ?? 0,
                DdoCode = bt.DdoCode,
                TreasuryCode = bt.TreasuryCode,
                Status = bt.Status
            }).ToList(),

            GstDetails = bill.GstDetails.Where(g => g.IsDeleted != true).Select(gst => new GstDetailDto
            {
                CpinId = gst.CpinId,
                DdoGstn = gst.DdoGstn,
                DdoCode = gst.DdoCode,
                TrId = gst.TrId
            }).ToList(),

            EcsDetails = bill.EcsDetails.Select(ecs => new EcsDetailDto
            {
                PayeeName = ecs.PayeeName,
                BeneficiaryId = ecs.BeneficiaryId,
                PanNo = ecs.PanNo,
                ContactNumber = ecs.ContactNumber,
                Address = ecs.Address,
                Email = ecs.Email,
                IfscCode = ecs.IfscCode,
                BankAccountNumber = ecs.BankAccountNumber,
                BankName = ecs.BankName,
                Amount = ecs.Amount ?? 0,
                IsGst = ecs.IsGst ?? false
            }).ToList(),

            AllotmentDetails = bill.AllotmentDetails.Select(allot => new AllotmentDetailDto
            {
                AllotmentId = allot.AllotmentId,
                Amount = allot.Amount,
                DdoCode = allot.DdoCode,
                TreasuryCode = allot.TreasuryCode,
                ActiveHoaId = allot.ActiveHoaId,
                AllotmentReceived = allot.AllotmentReceived,
                ProgressiveExpenses = allot.ProgressiveExpenses
            }).ToList(),

            SubvoucherDetails = bill.Subvouchers.Select(sv => new SubvoucherDetailDto
            {
                SubvoucherNo = sv.SubvoucherNo,
                SubvoucherDate = sv.SubvoucherDate,
                SubvoucherAmount = sv.SubvoucherAmount,
                Description = sv.Description
            }).ToList()
        };
    }

    public static FinancialYearDto ToFinancialYearDto(FinancialYearMaster fy)
    {
        return new FinancialYearDto
        {
            Id = fy.Id,
            FinancialYear = fy.FinancialYear,
            IsActive = fy.IsActive
        };
    }
}
