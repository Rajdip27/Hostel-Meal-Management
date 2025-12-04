using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Persistence.Configurations;

public class MealAttendanceConfiguration : IEntityTypeConfiguration<MealAttendance>
{
    public void Configure(EntityTypeBuilder<MealAttendance> builder)
    {
        builder.ToTable("MealAttendances");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.MealDate)
            .IsRequired();
        builder.HasOne(m => m.Member)
              .WithMany(b => b.MealAttendances)
              .HasForeignKey(m => m.MemberId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}

