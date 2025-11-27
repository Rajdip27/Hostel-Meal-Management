using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
       builder.ToTable(nameof(Member));
       builder.HasKey(x => x.Id);
       builder.Property(x => x.MemberCode).IsRequired();
       builder.Property(x => x.Email).IsRequired();
       builder.Property(x => x.PhoneNumber).IsRequired();
       builder.HasIndex(x=> x.MemberCode).IsUnique();
       builder.HasIndex(x=> x.Email).IsUnique();
       builder.HasIndex(x=> x.PhoneNumber).IsUnique();
       builder.HasIndex(x=> x.NID).IsUnique();
    }
}
