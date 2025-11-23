using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Infrastructure.Configuration.AuthConfiguration;

public class RoleConfiguration: IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new Role
        {
            Id = 1,
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",
            Description = "Default role assigned to all employees."

        }, new Role
        {
            Id = 2,
            Name = "Employee",
            NormalizedName = "EMPLOYEE",
            Description= "Default role assigned to all employees."
        });
    }
}
