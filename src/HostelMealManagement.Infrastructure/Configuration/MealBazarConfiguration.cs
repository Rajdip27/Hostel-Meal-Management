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

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.TotalDays)
            .IsRequired();

        builder.Property(x => x.MealMemberId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BazarAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Relationship: MealBazar -> MealBazarItems (One-to-Many)
        builder.HasMany(x => x.Items)
               .WithOne(x => x.MealBazar)
               .HasForeignKey(x => x.MealBazarId)
               .OnDelete(DeleteBehavior.Cascade);

        // OPTIONAL: If MealMemberId relates to Member entity
        // builder.HasOne(x => x.Member)
        //        .WithMany(x => x.MealBazars)
        //        .HasForeignKey(x => x.MealMemberId)
        //        .OnDelete(DeleteBehavior.Restrict);
    }
}
