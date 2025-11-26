using HostelMealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelMealManagement.Infrastructure.Configuration;

public class testConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable(nameof(Member));
        builder.HasKey(t => t.Id);
    }
}
