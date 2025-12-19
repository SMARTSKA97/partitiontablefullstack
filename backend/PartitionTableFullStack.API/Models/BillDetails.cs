using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_details", Schema = "billing")]
public class BillDetails
{
    [Column("bill_id")]
    public long BillId { get; set; }
    [Column("bill_no")]
    [StringLength(15)]
    public string? BillNo { get; set; }
    [Column("bill_date")]
    public DateOnly BillDate { get; set; }
    [Column("bill_mode")]
    public short? BillMode { get; set; }
    [Column("reference_no")]
    [StringLength(20)]
    public string? ReferenceNo { get; set; }
    [Column("tr_master_id")]
    public short TrMasterId { get; set; }
    [Column("payment_mode")]
    public short PaymentMode { get; set; }
    [Column("financial_year")]
    public short FinancialYear { get; set; }
    [Column("demand")]
    [StringLength(2)]
    public string? Demand { get; set; }
    [Column("major_head")]
    [StringLength(4)]
    public string? MajorHead { get; set; }
    [Column("sub_major_head")]
    [StringLength(2)]
    public string? SubMajorHead { get; set; }
    [Column("minor_head")]
    [StringLength(3)]
    public string? MinorHead { get; set; }
    [Column("ddo_code")]
    [StringLength(9)]
    public string? DdoCode { get; set; }
    [Column("treasury_code")]
    [StringLength(3)]
    public string? TreasuryCode { get; set; }
    [Column("gross_amount")]
    public long? GrossAmount { get; set; }
    [Column("net_amount")]
    public long? NetAmount { get; set; }
    [Column("bt_amount")]
    public long? BtAmount { get; set; }
    [Column("status")]
    public short Status { get; set; }
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
    [Column("is_gst")]
    public bool? IsGst { get; set; }
    [Column("gst_amount")]
    public long? GstAmount { get; set; }
    [Column("plan_status")]
    [StringLength(2)]
    public string? PlanStatus { get; set; }
    [Column("scheme_head")]
    [StringLength(3)]
    public string? SchemeHead { get; set; }
    [Column("detail_head")]
    [StringLength(2)]
    public string? DetailHead { get; set; }
    [Column("voted_charged")]
    [StringLength(1)]
    public string? VotedCharged { get; set; }
    [Column("remarks")]
    [StringLength(100)]
    public string? Remarks { get; set; }
    [Column("sanction_no")]
    [StringLength(50)]
    public string? SanctionNo { get; set; }
    [Column("sanction_by")]
    [StringLength(100)]
    public string? SanctionBy { get; set; }
    [Column("created_by_userid")]
    public long? CreatedByUserid { get; set; }
    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
    [Column("updated_by_userid")]
    public long? UpdatedByUserid { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
