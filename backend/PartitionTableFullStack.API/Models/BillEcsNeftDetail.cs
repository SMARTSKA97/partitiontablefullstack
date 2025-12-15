using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_ecs_neft_details", Schema = "billing")]
public class BillEcsNeftDetail
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [Column("financial_year")]
    public short FinancialYear { get; set; }

    [Column("payee_name")]
    [StringLength(100)]
    public string? PayeeName { get; set; }

    [Column("beneficiary_id")]
    [StringLength(100)]
    public string? BeneficiaryId { get; set; }

    [Column("pan_no")]
    [StringLength(10)]
    public string? PanNo { get; set; }

    [Column("contact_number")]
    [StringLength(15)]
    public string? ContactNumber { get; set; }

    [Column("address")]
    [StringLength(200)]
    public string? Address { get; set; }

    [Column("email")]
    [StringLength(60)]
    public string? Email { get; set; }

    [Column("ifsc_code")]
    [StringLength(11)]
    public string? IfscCode { get; set; }

    [Column("bank_account_number")]
    [StringLength(20)]
    public string? BankAccountNumber { get; set; }

    [Column("bank_name")]
    [StringLength(50)]
    public string? BankName { get; set; }

    [Column("amount")]
    public long? Amount { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("is_active")]
    public short? IsActive { get; set; }

    [Column("is_gst")]
    public bool? IsGst { get; set; }

    [Column("created_by_userid")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_by_userid")]
    public long? UpdatedByUserId { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    [ForeignKey("BillId,FinancialYear")]
    public BillDetail? Bill { get; set; }
}
