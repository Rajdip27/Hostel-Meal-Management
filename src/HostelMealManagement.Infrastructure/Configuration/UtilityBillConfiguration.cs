using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class UtilityBillConfiguration : IEntityTypeConfiguration<UtilityBill>
{
    public void Configure(EntityTypeBuilder<UtilityBill> builder)
    {
        builder.ToTable(nameof(UtilityBill));

        builder.HasKey(ub => ub.Id);

        builder.Property(ub => ub.Month)
               .IsRequired();

        builder.Property(ub => ub.Date)
               .IsRequired();

        builder.Property(ub => ub.CurrentBill)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(ub => ub.GasBill)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(ub => ub.ServantBill)
               .IsRequired()
               .HasColumnType("decimal(18,2)");
    }
}
