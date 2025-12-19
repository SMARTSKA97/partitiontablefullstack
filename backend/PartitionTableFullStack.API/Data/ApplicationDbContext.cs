using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveHoaMst> ActiveHoaMst { get; set; }

    public virtual DbSet<BillStatusMaster> BillStatusMaster { get; set; }

    public virtual DbSet<BtDetails> BtDetails { get; set; }

    public virtual DbSet<CpinMaster> CpinMaster { get; set; }

    public virtual DbSet<Ddo> Ddo { get; set; }

    public virtual DbSet<DdoAllotmentTransactions> DdoAllotmentTransactions { get; set; }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<FinancialYearMaster> FinancialYearMaster { get; set; }

    public virtual DbSet<RbiIfscStock> RbiIfscStock { get; set; }

    public virtual DbSet<TrMaster> TrMaster { get; set; }

    public virtual DbSet<Treasury> Treasury { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=billing_system;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveHoaMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("active_hoa_mst_pkey");

            entity.ToTable("active_hoa_mst", "master");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Demand)
                .HasMaxLength(2)
                .HasColumnName("demand");
            entity.Property(e => e.DetailHead)
                .HasMaxLength(2)
                .HasColumnName("detail_head");
            entity.Property(e => e.HoaDescription)
                .HasMaxLength(500)
                .HasColumnName("hoa_description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MajorHead)
                .HasMaxLength(4)
                .HasColumnName("major_head");
            entity.Property(e => e.MinorHead)
                .HasMaxLength(3)
                .HasColumnName("minor_head");
            entity.Property(e => e.PlanStatus)
                .HasMaxLength(2)
                .HasColumnName("plan_status");
            entity.Property(e => e.SchemeHead)
                .HasMaxLength(3)
                .HasColumnName("scheme_head");
            entity.Property(e => e.SubMajorHead)
                .HasMaxLength(2)
                .HasColumnName("sub_major_head");
            entity.Property(e => e.VotedCharged)
                .HasMaxLength(1)
                .HasColumnName("voted_charged");
        });

        modelBuilder.Entity<BillStatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("bill_status_master_pkey");

            entity.ToTable("bill_status_master", "billing_master");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusDescription)
                .HasMaxLength(200)
                .HasColumnName("status_description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<BtDetails>(entity =>
        {
            entity.HasKey(e => e.BtSerial).HasName("bt_details_pkey");

            entity.ToTable("bt_details", "billing_master");

            entity.Property(e => e.BtSerial)
                .ValueGeneratedNever()
                .HasColumnName("bt_serial");
            entity.Property(e => e.BtTypeName)
                .HasMaxLength(100)
                .HasColumnName("bt_type_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<CpinMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cpin_master_pkey");

            entity.ToTable("cpin_master", "billing_master");

            entity.HasIndex(e => e.CpinNumber, "cpin_master_cpin_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CpinDate).HasColumnName("cpin_date");
            entity.Property(e => e.CpinNumber)
                .HasMaxLength(20)
                .HasColumnName("cpin_number");
        });

        modelBuilder.Entity<Ddo>(entity =>
        {
            entity.HasKey(e => e.DdoCode).HasName("ddo_pkey");

            entity.ToTable("ddo", "master");

            entity.Property(e => e.DdoCode)
                .HasMaxLength(9)
                .HasColumnName("ddo_code");
            entity.Property(e => e.DdoName)
                .HasMaxLength(200)
                .HasColumnName("ddo_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<DdoAllotmentTransactions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ddo_allotment_transactions_pkey");

            entity.ToTable("ddo_allotment_transactions", "bantan", tb => tb.HasComment("DDO budget allotment tracking"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActiveHoaId).HasColumnName("active_hoa_id");
            entity.Property(e => e.AllotmentAmount).HasColumnName("allotment_amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DdoCode)
                .HasMaxLength(9)
                .HasColumnName("ddo_code");
            entity.Property(e => e.FinancialYear).HasColumnName("financial_year");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TreasuryCode)
                .HasMaxLength(3)
                .HasColumnName("treasury_code");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptCode).HasName("department_pkey");

            entity.ToTable("department", "master");

            entity.Property(e => e.DeptCode)
                .HasMaxLength(10)
                .HasColumnName("dept_code");
            entity.Property(e => e.DeptName)
                .HasMaxLength(200)
                .HasColumnName("dept_name");
        });

        modelBuilder.Entity<FinancialYearMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("financial_year_master_pkey");

            entity.ToTable("financial_year_master", "master", tb => tb.HasComment("Financial years (e.g., 2024-2025)"));

            entity.HasIndex(e => e.FinancialYear, "financial_year_master_financial_year_key").IsUnique();

            entity.HasIndex(e => e.FyCode, "financial_year_master_fy_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FinancialYear)
                .HasMaxLength(9)
                .HasColumnName("financial_year");
            entity.Property(e => e.FyCode)
                .HasMaxLength(4)
                .HasComment("Short code like 2425 for partition naming")
                .HasColumnName("fy_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(false)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<RbiIfscStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rbi_ifsc_stock_pkey");

            entity.ToTable("rbi_ifsc_stock", "master");

            entity.HasIndex(e => e.IfscCode, "rbi_ifsc_stock_ifsc_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .HasColumnName("branch_name");
            entity.Property(e => e.IfscCode)
                .HasMaxLength(11)
                .HasColumnName("ifsc_code");
        });

        modelBuilder.Entity<TrMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tr_master_pkey");

            entity.ToTable("tr_master", "billing_master");

            entity.HasIndex(e => e.TrName, "tr_master_tr_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TrName)
                .HasMaxLength(100)
                .HasColumnName("tr_name");
        });

        modelBuilder.Entity<Treasury>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("treasury_pkey");

            entity.ToTable("treasury", "master");

            entity.HasIndex(e => e.TreasuryCode, "treasury_treasury_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TreasuryCode)
                .HasMaxLength(3)
                .HasColumnName("treasury_code");
            entity.Property(e => e.TreasuryName)
                .HasMaxLength(100)
                .HasColumnName("treasury_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
