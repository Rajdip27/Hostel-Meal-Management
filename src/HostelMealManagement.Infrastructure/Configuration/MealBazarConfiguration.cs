using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Persistence.Configurations;

public class MealBazarConfiguration : IEntityTypeConfiguration<MealBazar>
{
    public void Configure(EntityTypeBuilder<MealBazar> builder)
    {
        builder.ToTable("MealBazars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BazarDate)
            .IsRequired();

        builder.Property(x => x.BazarAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired(false);

    }
}
