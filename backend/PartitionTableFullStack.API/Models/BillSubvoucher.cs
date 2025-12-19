using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_subvoucher", Schema = "billing")]
public class BillSubvoucher
{
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
    [StringLength(200)]
    public string? Description { get; set; }
    [Column("created_by_userid")]
    public long? CreatedByUserid { get; set; }
    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
    [Column("updated_by_userid")]
    public long? UpdatedByUserid { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
