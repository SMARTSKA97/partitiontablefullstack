using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_btdetail", Schema = "billing")]
public class BillBtDetail
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [Column("financial_year")]
    public short FinancialYear { get; set; }

    [Column("bt_serial")]
    public int? BtSerial { get; set; }

    [Column("bt_type")]
    public short? BtType { get; set; }

    [Column("amount")]
    public long? Amount { get; set; }

    [Column("ddo_code")]
    [StringLength(9)]
    public string? DdoCode { get; set; }

    [Column("treasury_code")]
    [StringLength(3)]
    public string? TreasuryCode { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    [ForeignKey("BillId,FinancialYear")]
    public BillDetail? Bill { get; set; }
}
