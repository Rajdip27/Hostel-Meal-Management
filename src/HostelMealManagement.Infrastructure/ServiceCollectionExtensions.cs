using HostelMealManagement.Infrastructure.DatabaseContext;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace HostelMealManagement.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Ensure logging services exist
        services.AddLogging();

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>((sp, builder) =>
        {
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings =>
                       warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            builder.UseLoggerFactory(loggerFactory);

            builder.LogTo(Console.WriteLine, LogLevel.Information);
        });

        // Register Identity
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = false;            // no digit required
            options.Password.RequiredLength = 1;             // minimum length
            options.Password.RequireNonAlphanumeric = false; // no special character required
            options.Password.RequireLowercase = false;       // no lowercase required
            options.Password.RequireUppercase = false;       // no uppercase required
        })
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        // Scoped SignInHelper
        services.AddTransient<ISignInHelper, SignInHelper>();

        // Authorization
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }
}
