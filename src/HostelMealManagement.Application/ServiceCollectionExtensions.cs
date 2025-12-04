using DinkToPdf;
using DinkToPdf.Contracts;
using HostelMealManagement.Application.FileServices;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.Mapping;
using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.Repositories.Auth;
using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.Services;
using HostelMealManagement.Application.Services.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HostelMealManagement.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IMealCycleRepository, MealCycleRepository>();
        services.AddScoped<IMealBazarRepository, MealBazarRepository>();
        services.AddScoped<IMealBazarRepository, MealBazarRepository>();
        services.AddScoped<IMealAttendanceRepository, MealAttendanceRepository>();
        services.AddScoped<IExcelUploadService, ExcelUploadService>();
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        // Register your PDF service
        services.AddScoped<IPdfService, PdfService>();
        // Register Razor view renderer
        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        services.AddAutoMapper(x => {
            x.AddMaps(typeof(IApplication).Assembly);

        });
    }
}
