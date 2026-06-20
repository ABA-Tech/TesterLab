using Auth.Core.Abstractions;
using Auth.Core.Extensions;
using Auth.Core.Services;
using Auth.JWT.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using TesterLab;
using TesterLab.Applications.Services;
using TesterLab.Data;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Infrastructure.Data;
using TesterLab.Infrastructure.Data.Repositories;
using TesterLab.Infrastructure.Selenium;
using TesterLab.JobScheduler.BackgroundServices;
using TesterLab.JobScheduler.Services;
using TesterLab.Rappory.Services;
using TesterLab.Repositories;
using TesterLab.Services;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════
// 1. HTTP CONTEXT
// ═══════════════════════════════════════════════════════
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// ═══════════════════════════════════════════════════════
// 2. BASES DE DONNÉES
// ═══════════════════════════════════════════════════════

if (builder.Environment.IsDevelopment())
{
  // Base de données principale (TesterLab)
  builder.Services.AddDbContext<TesterLabDbContext>(options =>
      options.UseSqlite("Data Source=app.db"));

  // Base de données utilisateurs (Auth)
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlite("Data Source=dbuser.db"));
}
else
{
  builder.Services.AddDbContext<TesterLabDbContext>(options =>
  {
    options.UseNpgsql(
      Environment.GetEnvironmentVariable("SUPERBASE_API_KEY"));
  });
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
  {
    options.UseNpgsql(
      Environment.GetEnvironmentVariable("SUPERBASE_API_KEY"));
  });
}



// ═══════════════════════════════════════════════════════
// 3. REPOSITORIES TESTERLAB
// ═══════════════════════════════════════════════════════
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
builder.Services.AddScoped<ITestRunRepository2, TestRunRepository2>();
builder.Services.AddScoped<IJobRepository2, JobRepository2>();
builder.Services.AddScoped<JobRepository>();

// ═══════════════════════════════════════════════════════
// 4. REPOSITORIES AUTH (AVANT AddMyCompanyAuthCore)
// ═══════════════════════════════════════════════════════
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();

// ═══════════════════════════════════════════════════════
// 5. SERVICES TESTERLAB
// ═══════════════════════════════════════════════════════
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ITestStepService, TestStepService>();
builder.Services.AddScoped<ITestCaseService, TestCaseService>();
builder.Services.AddScoped<ITestDataService, TestDataService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ITestExecutionService, TestExecutionService>();
builder.Services.AddScoped<ITestExecutionService2, TestExecutionService2>();
builder.Services.AddScoped<ITestExecutionService3, TestExecutionService3>();
builder.Services.AddScoped<IActionTemplateService, ActionTemplateService>();
builder.Services.AddScoped<ITestStepImportService, TestStepImportService>();
builder.Services.AddScoped<ITestSchedulerService, TestSchedulerService>();

// rapports

builder.Services.AddScoped<IReportDataService, ReportDataService>();
builder.Services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();
builder.Services.AddScoped<IHtmlReportGenerator, HtmlReportGenerator>();
builder.Services.AddScoped<IReportService, ReportService>();

// ═══════════════════════════════════════════════════════
// DOSSIER POUR LES RAPPORTS (wwwroot/reports)
// ═══════════════════════════════════════════════════════

var reportsPath = Path.Combine(builder.Environment.WebRootPath, "reports");
if (!Directory.Exists(reportsPath))
{
  Directory.CreateDirectory(reportsPath);
}


//builder.Services.AddScoped<TestRunExecutor>();
//builder.Services.AddSingleton<ITestRunQueue, TestRunQueue>();
//builder.Services.AddHostedService<TestRunBackgroundService>();
//builder.Services.AddScoped<TestRunExecutor>();
// Services génériques
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ═══════════════════════════════════════════════════════
// 6. SERVICES AUTH
// ═══════════════════════════════════════════════════════
// Configuration Email
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddHttpClient<ResendClient>();

builder.Services.Configure<ResendClientOptions>(o =>
{
  o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY");
});

builder.Services.AddTransient<IResend, ResendClient>();

// Services de rôles
builder.Services.AddScoped<IRoleService, RoleService>();

// Services d'authentification de base (PasswordHasher, Validator)
builder.Services.AddMyCompanyAuthCore(builder.Configuration);

// Services d'authentification et utilisateur
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();

// Service JWT (enregistre ITokenService)
builder.Services.AddMyCompanyAuthWithJwt(builder.Configuration);

// ═══════════════════════════════════════════════════════
// 7. AUTHENTIFICATION (COOKIE PAR DÉFAUT POUR MVC)
// ═══════════════════════════════════════════════════════
builder.Services.AddAuthentication(options =>
{
  // ✅ Cookie comme schéma par défaut pour MVC
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
  options.LoginPath = "/Account/Login";
  options.LogoutPath = "/Account/Logout";
  options.AccessDeniedPath = "/Account/AccessDenied";
  options.ExpireTimeSpan = TimeSpan.FromHours(12);
  options.SlidingExpiration = true;
  options.Cookie.HttpOnly = true;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.Cookie.SameSite = SameSiteMode.Strict;
  options.Cookie.Name = "TesterLab.Auth";
});

// ═══════════════════════════════════════════════════════
// 8. AUTORISATION
// ═══════════════════════════════════════════════════════
builder.Services.AddAuthorization(options =>
{
  // Politiques personnalisées
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
  options.AddPolicy("ModeratorOrAdmin", policy => policy.RequireRole("Admin", "Moderator"));
});

// ═══════════════════════════════════════════════════════
// 9. MVC & SIGNALR
// ═══════════════════════════════════════════════════════
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// ═══════════════════════════════════════════════════════
// 10. BACKGROUND SERVICES
// ═══════════════════════════════════════════════════════
builder.Services.AddHostedService<JobSchedulerService>();

// ═══════════════════════════════════════════════════════
// 11. SÉCURITÉ
// ═══════════════════════════════════════════════════════
builder.Services.AddHsts(options =>
{
  options.Preload = true;
  options.IncludeSubDomains = true;
  options.MaxAge = TimeSpan.FromDays(365);
});

// ═══════════════════════════════════════════════════════
// SERVICES DE PARAMÈTRES SYSTÈME
// ═══════════════════════════════════════════════════════

builder.Services.AddScoped<ISystemSettingsService, SystemSettingsRepository>();

var app = builder.Build();

// ═══════════════════════════════════════════════════════
// MIDDLEWARE
// ═══════════════════════════════════════════════════════
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
  app.UseHttpsRedirection();
}
else
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

// ✅ ORDRE CRITIQUE : Authentication AVANT Authorization
app.UseAuthentication();
app.UseAuthorization();

// SignalR Hub
app.MapHub<TesterLab.Hubs.TestCaseHub>("/hubs/testcase");

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

// ═══════════════════════════════════════════════════════
// INITIALISATION DES BASES DE DONNÉES
// ═══════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var logger = services.GetRequiredService<ILogger<Program>>();

  try
  {
    // Initialiser la base TesterLab
    var testerLabContext = services.GetRequiredService<TesterLabDbContext>();
    testerLabContext.Database.Migrate();
    logger.LogInformation("✅ Base de données TesterLab initialisée");

    // Initialiser la base Auth
    var authContext = services.GetRequiredService<ApplicationDbContext>();
    authContext.Database.Migrate();
    logger.LogInformation("✅ Base de données Auth initialisée");
  }
  catch (Exception ex)
  {
    logger.LogError(ex, "❌ Erreur lors de l'initialisation des bases de données");
    throw;
  }
}

app.Run();
