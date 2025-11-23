using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;
namespace HostelMealManagement.Infrastructure.Configuration.AuthConfiguration;
public class UserRoleConfiguration: IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(new UserRole
        {
            RoleId = 1,
            UserId = 1,
        }, new UserRole
        {
            RoleId = 2,
            UserId = 2,
        });
    }
}
