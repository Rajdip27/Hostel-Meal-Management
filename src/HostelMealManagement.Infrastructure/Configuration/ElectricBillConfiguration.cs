using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Persistence.Configurations;

public class ElectricBillConfiguration : IEntityTypeConfiguration<ElectricBill>
{
    public void Configure(EntityTypeBuilder<ElectricBill> builder)
    {
        builder.ToTable("ElectricBills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MealCycleId)
               .IsRequired();

        builder.Property(x => x.PreviousUnit)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(x => x.CurrentUnit)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(x => x.PerUnitRate)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.HasOne(x => x.MealCycle)
               .WithMany()
               .HasForeignKey(x => x.MealCycleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
