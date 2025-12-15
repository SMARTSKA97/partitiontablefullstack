using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_subvoucher", Schema = "billing")]
public class BillSubvoucher
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [Column("financial_year")]
    public short FinancialYear { get; set; }

    [Column("subvoucher_no")]
    [StringLength(50)]
    public string? SubvoucherNo { get; set; }

    [Column("subvoucher_date")]
    public DateOnly? SubvoucherDate { get; set; }

    [Column("subvoucher_amount")]
    public long? SubvoucherAmount { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

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
