using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class UtilityBillConfiguration : IEntityTypeConfiguration<UtilityBill>
{
    public void Configure(EntityTypeBuilder<UtilityBill> builder)
    {
        builder.ToTable(nameof(UtilityBill));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UtilityType)
               .IsRequired()
               .HasConversion<int>();

        // ========================
        // Billing Period
        // ========================

        builder.Property(x => x.BillYear)
               .IsRequired();

        builder.Property(x => x.BillMonth)
               .IsRequired();

        builder.Property(x => x.BillDate)
               .IsRequired();

        // ========================
        // Electric only
        // ========================

        builder.Property(x => x.CurrentUnit)
               .HasPrecision(18, 2);

        builder.Property(x => x.PerUnitRate)
               .HasPrecision(18, 2);

        // ========================
        // Common for all utilities
        // ========================

        builder.Property(x => x.TotalUnit)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        // ========================
        // Indexes
        // ========================

        // For fast monthly reports
        builder.HasIndex(x => new { x.BillYear, x.BillMonth });

        // Prevent duplicate utility bill for same month
        builder.HasIndex(x => new { x.UtilityType, x.BillYear, x.BillMonth })
               .IsUnique();
    }
}

