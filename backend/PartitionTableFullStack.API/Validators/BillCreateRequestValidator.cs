using FluentValidation;
using PartitionTableFullStack.API.DTOs;

namespace PartitionTableFullStack.API.Validators;

public class BillCreateRequestValidator : AbstractValidator<BillCreateRequestDto>
{
    public BillCreateRequestValidator()
    {
        // Basic Bill Information
        RuleFor(x => x.BillDate)
            .NotEmpty().WithMessage("Bill date is required")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Bill date cannot be in the future");

        RuleFor(x => x.DdoCode)
            .NotEmpty().WithMessage("DDO Code is required")
            .MaximumLength(50).WithMessage("DDO Code must not exceed 50 characters");

        RuleFor(x => x.TreasuryCode)
            .NotEmpty().WithMessage("Treasury Code is required")
            .MaximumLength(50).WithMessage("Treasury Code must not exceed 50 characters");

        RuleFor(x => x.TrMasterId)
            .Must(id => id > 0).WithMessage("TR Master ID must be greater than 0");

        // Financial Details
        RuleFor(x => x.GrossAmount)
            .Must(x => x > 0).WithMessage("Gross amount must be greater than 0")
            .Must(x => x <= 99999999999.99m).WithMessage("Gross amount exceeds maximum limit");

        RuleFor(x => x.NetAmount)
            .Must(x => x > 0).WithMessage("Net amount must be greater than 0")
            .Must(x => x <= 99999999999.99m).WithMessage("Net amount exceeds maximum limit");

        RuleFor(x => x.BtAmount)
            .Must(x => x >= 0).WithMessage("BT amount cannot be negative")
            .Must(x => x <= 99999999999.99m).WithMessage("BT amount exceeds maximum limit");

        RuleFor(x => x.GstAmount)
            .Must(x => x >= 0).WithMessage("GST amount cannot be negative")
            .When(x => x.IsGst == true);

        // Business Rule: Gross Amount Calculation
        // If GST: gross = net + bt + gst, else: gross = net + bt
        RuleFor(x => x)
            .Must(ValidateGrossAmount)
            .WithMessage("Gross amount calculation is incorrect. If GST: gross = net + bt + gst, else: gross = net + bt");

        // BT Details Validation
        RuleFor(x => x.BtDetails)
            .NotEmpty().WithMessage("At least one BT detail is required");

        RuleForEach(x => x.BtDetails).ChildRules(bt =>
        {
            bt.RuleFor(x => x.Amount)
                .Must(a => a > 0).WithMessage("BT detail amount must be greater than 0");
        });

        // Business Rule: Sum of BT details must equal total BT amount
        RuleFor(x => x)
            .Must(ValidateBtDetailsSum)
            .WithMessage("Sum of individual BT amounts must equal total BT amount");

        // GST Details Validation
        RuleForEach(x => x.GstDetails).ChildRules(gst =>
        {
            gst.RuleFor(x => x.DdoGstn)
                .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
                .When(x => !string.IsNullOrEmpty(x.DdoGstn))
                .WithMessage("Invalid GSTIN format (e.g., 29ABCDE1234F1Z5)");
        });

        // ECS/NEFT Details Validation
        RuleFor(x => x.EcsDetails)
            .NotEmpty().WithMessage("At least one ECS/NEFT detail is required");

        RuleForEach(x => x.EcsDetails).ChildRules(ecs =>
        {
            ecs.RuleFor(x => x.PayeeName)
                .NotEmpty().WithMessage("Payee name is required")
                .MaximumLength(200).WithMessage("Payee name must not exceed 200 characters");

            ecs.RuleFor(x => x.BankAccountNumber)
                .NotEmpty().WithMessage("Bank account number is required")
                .Matches(@"^\d{9,18}$").WithMessage("Invalid bank account number (9-18 digits required)");

            ecs.RuleFor(x => x.IfscCode)
                .NotEmpty().WithMessage("IFSC code is required")
                .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$").WithMessage("Invalid IFSC code format (e.g., SBIN0001234)");

            ecs.RuleFor(x => x.Amount)
                .Must(a => a > 0).WithMessage("ECS amount must be greater than 0");
        });

        // Business Rule: Sum of ECS amounts must equal net amount
        RuleFor(x => x)
            .Must(ValidateEcsDetailsSum)
            .WithMessage("Sum of individual ECS amounts must equal net amount");

        // Allotment Details Validation
        RuleFor(x => x.AllotmentDetails)
            .NotEmpty().WithMessage("At least one allotment detail is required");

        RuleForEach(x => x.AllotmentDetails).ChildRules(allot =>
        {
            allot.RuleFor(x => x.Amount)
                .Must(a => a > 0).WithMessage("Allotment amount must be greater than 0");

            allot.RuleFor(x => x.ActiveHoaId)
                .Must(id => id > 0).WithMessage("Active HOA ID must be greater than 0");
        });

        // Business Rule: Total allotment amount must be >= gross amount
        RuleFor(x => x)
            .Must(ValidateAllotmentAmount)
            .WithMessage("Total allotment amount must be greater than or equal to gross amount");
    }

    private bool ValidateGrossAmount(BillCreateRequestDto dto)
    {
        var netAmount = dto.NetAmount;
        var btAmount = dto.BtAmount;
        var gstAmount = dto.GstAmount ?? 0;
        var grossAmount = dto.GrossAmount;

        long expectedGross;
        if (dto.IsGst == true)
        {
            // If GST: gross = net + bt + gst
            expectedGross = netAmount + btAmount + gstAmount;
        }
        else
        {
            // Else: gross = net + bt
            expectedGross = netAmount + btAmount;
        }

        // Exact match required for long amounts
        return grossAmount == expectedGross;
    }

    private bool ValidateBtDetailsSum(BillCreateRequestDto dto)
    {
        var totalBtDetails = dto.BtDetails?.Sum(bt => bt.Amount) ?? 0;
        var btAmount = dto.BtAmount;

        // Exact match required
        return totalBtDetails == btAmount;
    }

    private bool ValidateEcsDetailsSum(BillCreateRequestDto dto)
    {
        var totalEcsAmount = dto.EcsDetails?.Sum(e => e.Amount) ?? 0;
        var netAmount = dto.NetAmount;
        
        // Exact match required
        return totalEcsAmount == netAmount;
    }

    private bool ValidateAllotmentAmount(BillCreateRequestDto dto)
    {
        var totalAllotmentAmount = dto.AllotmentDetails?.Sum(a => a.Amount) ?? 0;
        var grossAmount = dto.GrossAmount;

        // Total allotment must be >= gross amount
        return totalAllotmentAmount >= grossAmount;
    }
}
