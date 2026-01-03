using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class MealBillConfiguration : IEntityTypeConfiguration<MealBill>
{
    public void Configure(EntityTypeBuilder<MealBill> builder)
    {
        builder.ToTable(nameof(MealBill));
        builder.HasKey(m => m.Id);
        builder.HasOne(m => m.Member)
               .WithMany(b => b.MealBills)
               .HasForeignKey(m => m.MemberId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.MealCycle)
               .WithMany(b => b.MealBills)
               .HasForeignKey(m => m.MealCycleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
