using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using TesterLab;
using TesterLab.Applications.Services;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Infrastructure.Data;
using TesterLab.Infrastructure.Data.Repositories;
using TesterLab.Infrastructure.Selenium;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<TesterLabDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Repositories
builder.Services.AddScoped<ITestExecutor, SeleniumTestExecutor>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
builder.Services.AddScoped<ITestRunRepository, TestRunRepository>();
builder.Services.AddScoped<ITestCaseRepository, TestCaseRepository>();
builder.Services.AddScoped<ITestStepRepository, TestStepRepository>();
builder.Services.AddScoped<ITestDataRepository, TestDataRepository>();
builder.Services.AddScoped<IScreenshotRepository, ScreenshotRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IExecutionLogRepository, ExecutionLogRepository>();
builder.Services.AddScoped<IActionTemplateRepository, ActionTemplateRepository>();
builder.Services.AddScoped<ITestStepExecutionRepository, TestStepExecutionRepository>();
builder.Services.AddScoped<ITestCaseExecutionRepository, TestCaseExecutionRepository>();
builder.Services.AddScoped<IPerformanceMetricRepository, PerformanceMetricRepository>();

// ... tous les autres

// Services
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ITestStepService, TestStepService>();
builder.Services.AddScoped<ITestCaseService, TestCaseService>();
builder.Services.AddScoped<ITestDataService, TestDataService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ITestExecutionService, TestExecutionService>();
builder.Services.AddScoped<ITestExecutionService2, TestExecutionService2>();;
builder.Services.AddScoped<ITestExecutionService3, TestExecutionService3>();
builder.Services.AddScoped<IActionTemplateService, ActionTemplateService>();

// Génériques
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

app.Run();
