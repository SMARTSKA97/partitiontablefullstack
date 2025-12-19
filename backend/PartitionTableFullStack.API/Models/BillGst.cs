using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("bill_gst", Schema = "billing")]
public class BillGst
{
    [Column("id")]
    public long Id { get; set; }
    [Column("bill_id")]
    public long BillId { get; set; }
    [Column("financial_year")]
    public short FinancialYear { get; set; }
    [Column("cpin_id")]
    public int? CpinId { get; set; }
    [Column("ddo_gstn")]
    [StringLength(15)]
    public string? DdoGstn { get; set; }
    [Column("created_by_userid")]
    public long? CreatedByUserid { get; set; }
    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
    [Column("updated_by_userid")]
    public long? UpdatedByUserid { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
