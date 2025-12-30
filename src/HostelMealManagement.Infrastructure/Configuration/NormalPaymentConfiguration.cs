using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Persistence.Configurations;

public class NormalPaymentConfiguration : IEntityTypeConfiguration<NormalPayment>
{
    public void Configure(EntityTypeBuilder<NormalPayment> builder)
    {
        builder.ToTable("NormalPayments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PaymentDate)
               .IsRequired();

        builder.Property(x => x.MemberId)
               .IsRequired();

        builder.Property(x => x.PaymentAmount)
               .IsRequired()
               .HasPrecision(18, 2);


        builder.Property(x => x.Remarks)
               .HasMaxLength(500);

    
         builder.HasOne<Member>()
                .WithMany(x=>x.NormalPayment)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
