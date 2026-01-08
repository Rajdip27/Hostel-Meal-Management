using HostelMealManagement.Application;
using HostelMealManagement.Application.CommonModel;   // ✅ ADD
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using HostelMealManagement.Infrastructure;
using HostelMealManagement.Infrastructure.Services;   // ✅ ADD
using HostelMealManagement.Services;                  // ✅ ADD
using HostelMealManagement.Web.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<TraceIdEnricher>()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Services.Configure<SSLCommerzOptions>(
    builder.Configuration.GetSection("SSLCommerz"));

builder.Host.UseSerilog();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

// ===================== ADD START =====================
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, GmailEmailService>();
// ===================== ADD END =====================

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
