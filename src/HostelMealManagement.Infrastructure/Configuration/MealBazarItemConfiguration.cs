using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class MealBazarItemConfiguration : IEntityTypeConfiguration<MealBazarItem>
{
    public void Configure(EntityTypeBuilder<MealBazarItem> builder)
    {
        builder.ToTable(nameof(MealBazarItem));
        builder.HasKey(m => m.Id);
        builder.HasOne(m => m.MealBazar)
               .WithMany(b => b.Items)
               .HasForeignKey(m => m.MealBazarId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
