using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class MealMenuConfiguration : IEntityTypeConfiguration<MealMenu>
{
    public void Configure(EntityTypeBuilder<MealMenu> builder)
    {
        // Table Name
        builder.ToTable(nameof(MealMenu));

        // Primary Key
        builder.HasKey(m => m.Id);

        // DayOfWeek enum saved as int
        builder.Property(m => m.DayOfWeek)
               .IsRequired()
               .HasConversion<int>();

        // Meal Name (Breakfast / Lunch / Dinner)
        builder.Property(m => m.MealName)
               .IsRequired()
               .HasMaxLength(100);

        // Menu items (food list)
        builder.Property(m => m.MenuItems)
               .IsRequired()
               .HasMaxLength(500);

        // Active / Inactive
        builder.Property(m => m.IsActive)
               .IsRequired();
    }
}
