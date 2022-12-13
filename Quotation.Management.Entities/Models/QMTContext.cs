using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Quotation.Management.Entities.Models
{
    public partial class QMTContext : DbContext
    {
        public QMTContext()
        {
        }

        public QMTContext(DbContextOptions<QMTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BrandMaster> BrandMasters { get; set; } = null!;
        public virtual DbSet<CostItemCode> CostItemCodes { get; set; } = null!;
        public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; } = null!;
        public virtual DbSet<CustomerMaster> CustomerMasters { get; set; } = null!;
        public virtual DbSet<DeliveryTermMaster> DeliveryTermMasters { get; set; } = null!;
        public virtual DbSet<ItemGroupMaster> ItemGroupMasters { get; set; } = null!;
        public virtual DbSet<ItemMaster> ItemMasters { get; set; } = null!;
        public virtual DbSet<OptionMaster> OptionMasters { get; set; } = null!;
        public virtual DbSet<PaymentTermMaster> PaymentTermMasters { get; set; } = null!;
        public virtual DbSet<PricingMaster> PricingMasters { get; set; } = null!;
        public virtual DbSet<ProductMaster> ProductMasters { get; set; } = null!;
        public virtual DbSet<QuotationCostItem> QuotationCostItems { get; set; } = null!;
        public virtual DbSet<QuotationCostItemLine> QuotationCostItemLines { get; set; } = null!;
        public virtual DbSet<QuotationHeader> QuotationHeaders { get; set; } = null!;
        public virtual DbSet<QuotationLine> QuotationLines { get; set; } = null!;
        public virtual DbSet<QuotationOptCode> QuotationOptCodes { get; set; } = null!;
        public virtual DbSet<QuotationStatusMaster> QuotationStatusMasters { get; set; } = null!;
        public virtual DbSet<SalesArea> SalesAreas { get; set; } = null!;
        public virtual DbSet<SeriesMaster> SeriesMasters { get; set; } = null!;
        public virtual DbSet<SeriesOption> SeriesOptions { get; set; } = null!;
        public virtual DbSet<UserMaster> UserMasters { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;User ID=admin;Password=1234567;Database=QMT;Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandMaster>(entity =>
            {
                entity.HasKey(e => e.BrandId)
                    .HasName("PK__BrandMas__DAD4F05EB5BB8FDF");

                entity.ToTable("BrandMaster");

                entity.Property(e => e.BrandName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.BrandMasters)
                    .HasForeignKey(d => d.CurrencyCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BrandMast__Curre__5CA1C101");
            });

            modelBuilder.Entity<CostItemCode>(entity =>
            {
                entity.HasKey(e => e.CostItemId)
                    .HasName("PK__CostItem__F36A96DC0C54B2C8");

                entity.Property(e => e.CostItemId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CurrencyMaster>(entity =>
            {
                entity.HasKey(e => e.CurrencyCode)
                    .HasName("PK__Currency__408426BEC29F5A52");

                entity.ToTable("CurrencyMaster");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ConvFactor).HasColumnType("decimal(6, 2)");
            });

            modelBuilder.Entity<CustomerMaster>(entity =>
            {
                entity.HasKey(e => e.CustomerCode)
                    .HasName("PK__Customer__06678520B581B887");

                entity.ToTable("CustomerMaster");

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DeliveryTermMaster>(entity =>
            {
                entity.ToTable("DeliveryTermMaster");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DeliveryTermName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ItemGroupMaster>(entity =>
            {
                entity.HasKey(e => e.GroupId)
                    .HasName("PK__ItemGrou__149AF36A6ED67E6D");

                entity.ToTable("ItemGroupMaster");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProdTypeId)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProdType)
                    .WithMany(p => p.ItemGroupMasters)
                    .HasForeignKey(d => d.ProdTypeId)
                    .HasConstraintName("FK__ItemGroup__ProdT__30F848ED");
            });

            modelBuilder.Entity<ItemMaster>(entity =>
            {
                entity.HasKey(e => e.ItemCode)
                    .HasName("PK__ItemMast__3ECC0FEBC4D15289");

                entity.ToTable("ItemMaster");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ItemCodeDescription)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Series)
                    .WithMany(p => p.ItemMasters)
                    .HasForeignKey(d => d.SeriesId)
                    .HasConstraintName("FK__ItemMaste__Serie__398D8EEE");
            });

            modelBuilder.Entity<OptionMaster>(entity =>
            {
                entity.HasKey(e => e.OptCode)
                    .HasName("PK__OptionMa__A1FAFBECDF7C609C");

                entity.ToTable("OptionMaster");

                entity.Property(e => e.OptCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OptName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentTermMaster>(entity =>
            {
                entity.ToTable("PaymentTermMaster");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PaymentTermName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PricingMaster>(entity =>
            {
                entity.HasKey(e => new { e.ItemCode, e.OptCode, e.Version });

                entity.ToTable("PricingMaster");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OptCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Version)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Version_");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Status_");

                entity.HasOne(d => d.OptCodeNavigation)
                    .WithMany(p => p.PricingMasters)
                    .HasForeignKey(d => d.OptCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PricingMa__OptCo__4222D4EF");
            });

            modelBuilder.Entity<ProductMaster>(entity =>
            {
                entity.HasKey(e => e.ProdTypeId)
                    .HasName("PK__ProductM__C9919688D1F73F0E");

                entity.ToTable("ProductMaster");

                entity.Property(e => e.ProdTypeId)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ProdName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QuotationCostItem>(entity =>
            {
                entity.HasKey(e => e.QuotationCostItemGroupId)
                    .HasName("PK__Quotatio__206CF075B6AF2940");

                entity.ToTable("QuotationCostItem");

                entity.Property(e => e.QuotationCostItemGroupId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FreightRate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProdTypeId)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.QuotationNum)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.CostItem)
                    .WithMany(p => p.QuotationCostItems)
                    .HasForeignKey(d => d.CostItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__CostI__1D7B6025");

                entity.HasOne(d => d.ProdType)
                    .WithMany(p => p.QuotationCostItems)
                    .HasForeignKey(d => d.ProdTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__ProdT__1C873BEC");

                entity.HasOne(d => d.QuotationHeader)
                    .WithMany(p => p.QuotationCostItems)
                    .HasForeignKey(d => new { d.QuotationNum, d.RevNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuotationCostIte__1B9317B3");
            });

            modelBuilder.Entity<QuotationCostItemLine>(entity =>
            {
                entity.HasKey(e => new { e.QuotationCostItemGroupId, e.LineNum })
                    .HasName("PK__Quotatio__B77E6D82CB74D473");

                entity.Property(e => e.QuotationCostItemGroupId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemLineValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.QuotationNum)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.QuotationCostItemGroup)
                    .WithMany(p => p.QuotationCostItemLines)
                    .HasForeignKey(d => d.QuotationCostItemGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Quota__214BF109");

                entity.HasOne(d => d.QuotationLine)
                    .WithMany(p => p.QuotationCostItemLines)
                    .HasForeignKey(d => new { d.QuotationNum, d.RevNum, d.LineNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuotationCostIte__2057CCD0");
            });

            modelBuilder.Entity<QuotationHeader>(entity =>
            {
                entity.HasKey(e => new { e.QuotationNum, e.RevNum })
                    .HasName("PK__Quotatio__DDA72342E9A10DC2");

                entity.ToTable("QuotationHeader");

                entity.Property(e => e.QuotationNum)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AreaCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BookingDate).HasColumnType("datetime");

                entity.Property(e => e.ConvFactor).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");

                entity.Property(e => e.Msp).HasColumnName("MSP");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.QuotationDate).HasColumnType("datetime");

                entity.HasOne(d => d.AreaCodeNavigation)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.AreaCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__AreaC__2645B050");

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.CurrencyCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Curre__245D67DE");

                entity.HasOne(d => d.CustomerCodeNavigation)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.CustomerCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Custo__236943A5");

                entity.HasOne(d => d.DeliveryTerm)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.DeliveryTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Deliv__2739D489");

                entity.HasOne(d => d.MspNavigation)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.Msp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuotationHe__MSP__25518C17");

                entity.HasOne(d => d.PaymentTerm)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.PaymentTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Payme__282DF8C2");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.QuotationHeaders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Statu__29221CFB");
            });

            modelBuilder.Entity<QuotationLine>(entity =>
            {
                entity.HasKey(e => new { e.QuotationNum, e.RevNum, e.LineNum })
                    .HasName("PK__Quotatio__A4D60A9D8FFE6FF9");

                entity.Property(e => e.QuotationNum)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CostItemLineValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mtlp).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SubItemCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Vat)
                    .HasColumnType("decimal(6, 2)")
                    .HasColumnName("VAT");

                entity.HasOne(d => d.ItemCodeNavigation)
                    .WithMany(p => p.QuotationLines)
                    .HasForeignKey(d => d.ItemCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__ItemC__2CF2ADDF");

                entity.HasOne(d => d.QuotationHeader)
                    .WithMany(p => p.QuotationLines)
                    .HasForeignKey(d => new { d.QuotationNum, d.RevNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuotationLines__2BFE89A6");
            });

            modelBuilder.Entity<QuotationOptCode>(entity =>
            {
                entity.HasKey(e => new { e.QuotationNum, e.RevNum, e.LineNum, e.OptCode })
                    .HasName("PK__Quotatio__1A0C15328CDCB14A");

                entity.ToTable("QuotationOptCode");

                entity.Property(e => e.QuotationNum)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OptCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OptName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OptType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.QuotationLine)
                    .WithMany(p => p.QuotationOptCodes)
                    .HasForeignKey(d => new { d.QuotationNum, d.RevNum, d.LineNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuotationOptCode__2FCF1A8A");
            });

            modelBuilder.Entity<QuotationStatusMaster>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__Quotatio__C8EE2063E8F7EA91");

                entity.ToTable("QuotationStatusMaster");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.StatusName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SalesArea>(entity =>
            {
                entity.HasKey(e => e.AreaCode)
                    .HasName("PK__SalesAre__72299A26A9E6918C");

                entity.ToTable("SalesArea");

                entity.Property(e => e.AreaCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AreaName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Frequency)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SeriesMaster>(entity =>
            {
                entity.HasKey(e => e.SeriesId)
                    .HasName("PK__SeriesMa__F3A1C1610927F250");

                entity.ToTable("SeriesMaster");

                entity.Property(e => e.Frequency).HasMaxLength(20);

                entity.Property(e => e.ParentSeries)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SeriesName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.SeriesMasters)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK__SeriesMas__Brand__36B12243");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.SeriesMasters)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK__SeriesMas__Group__35BCFE0A");
            });

            modelBuilder.Entity<SeriesOption>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.OptCode, e.SeriesId }, "UC_SeriesOptions")
                    .IsUnique();

                entity.Property(e => e.OptCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.OptCodeNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OptCode)
                    .HasConstraintName("FK__SeriesOpt__OptCo__3F466844");

                entity.HasOne(d => d.Series)
                    .WithMany()
                    .HasForeignKey(d => d.SeriesId)
                    .HasConstraintName("FK__SeriesOpt__Serie__3E52440B");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.ToTable("UserMaster");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("lastName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
