using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class MealCycleConfiguration : IEntityTypeConfiguration<MealCycle>
{
    public void Configure(EntityTypeBuilder<MealCycle> builder)
    {
        builder.ToTable(nameof(MealCycle));
        builder.HasKey(mc => mc.Id);
        builder.Property(mc => mc.Name)
               .IsRequired()
               .HasMaxLength(100).IsRequired();
        builder.Property(mc => mc.StartDate)
               .IsRequired();
        builder.Property(mc => mc.EndDate)
               .IsRequired();
    }
}
