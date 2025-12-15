using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_gst", Schema = "billing")]
public class BillGst
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [Column("financial_year")]
    public short FinancialYear { get; set; }

    [Column("cpin_id")]
    public long? CpinId { get; set; }

    [Column("ddo_gstn")]
    [StringLength(255)]
    public string? DdoGstn { get; set; }

    [Column("ddo_code")]
    [StringLength(9)]
    public string? DdoCode { get; set; }

    [Column("tr_id")]
    public short? TrId { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

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
