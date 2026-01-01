using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable(nameof(PaymentTransaction));
        builder.HasKey(x => x.Id);


        builder.HasOne(x => x.Member)
                .WithMany(x => x.PaymentTransaction)
               .HasForeignKey(x => x.MemberId);
        builder.HasOne(x => x.MealCycle)
               .WithMany(x => x.PaymentTransaction)
              .HasForeignKey(x => x.MealCycleId);

        builder.HasOne(x => x.MealBill)
               .WithMany(x => x.PaymentTransaction)
              .HasForeignKey(x => x.MealBillId);

    }
}
