using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartitionTableFullStack.API.Models;

[Table("financial_year_master", Schema = "master")]
public class FinancialYearMaster
{
    [Key]
    [Column("id")]
    public short Id { get; set; }

    [Column("financial_year")]
    [StringLength(9)]
    public string FinancialYear { get; set; } = string.Empty;

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_by_userid")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_by_userid")]
    public long? UpdatedByUserId { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
