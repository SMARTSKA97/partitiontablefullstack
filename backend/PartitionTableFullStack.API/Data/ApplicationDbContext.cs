using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Master tables
    public DbSet<FinancialYearMaster> FinancialYears { get; set; }

    // Billing tables (partitioned)
    public DbSet<BillDetail> BillDetails { get; set; }
    public DbSet<BillBtDetail> BillBtDetails { get; set; }
    public DbSet<BillGst> BillGsts { get; set; }
    public DbSet<BillEcsNeftDetail> BillEcsNeftDetails { get; set; }
    public DbSet<DdoAllotmentBookedBill> DdoAllotmentBookedBills { get; set; }
    public DbSet<BillSubvoucher> BillSubvouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure BillDetail
        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => new { e.BillId, e.FinancialYear });
            
entity.Property(e => e.BillId)
                .ValueGeneratedOnAdd();

            // Relationships
            entity.HasMany(e => e.BtDetails)
                .WithOne(e => e.Bill)
                .HasForeignKey(e => new { e.BillId, e.FinancialYear })
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.GstDetails)
                .WithOne(e => e.Bill)
                .HasForeignKey(e => new { e.BillId, e.FinancialYear })
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.EcsDetails)
                .WithOne(e => e.Bill)
                .HasForeignKey(e => new { e.BillId, e.FinancialYear })
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AllotmentDetails)
                .WithOne(e => e.Bill)
                .HasForeignKey(e => new { e.BillId, e.FinancialYear })
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Subvouchers)
                .WithOne(e => e.Bill)
                .HasForeignKey(e => new { e.BillId, e.FinancialYear })
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure BillBtDetail
        modelBuilder.Entity<BillBtDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.FinancialYear });
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });

        // Configure BillGst
        modelBuilder.Entity<BillGst>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.FinancialYear });
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });

        // Configure BillEcsNeftDetail
        modelBuilder.Entity<BillEcsNeftDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.FinancialYear });
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });

        // Configure DdoAllotmentBookedBill
        modelBuilder.Entity<DdoAllotmentBookedBill>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.FinancialYear });
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });

        // Configure BillSubvoucher
        modelBuilder.Entity<BillSubvoucher>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.FinancialYear });
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });
    }
}
