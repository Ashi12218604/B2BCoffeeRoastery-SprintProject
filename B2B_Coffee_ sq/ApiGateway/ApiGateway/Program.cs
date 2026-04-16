using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using B2B.Common.Middlewares;
using Serilog;
using System.Text;

// ── Configure Serilog ─────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/api-gateway-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting API Gateway...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

// ── Load Ocelot config files ──────────────────────────────────────────────
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.swagger.json", optional: false, reloadOnChange: true);

// ── JWT Authentication ────────────────────────────────────────────────────
// Gateway validates JWT so downstream services can trust forwarded claims
var jwt = builder.Configuration.GetSection("JwtSettings");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Secret"]!))
        };
    });

// ── CORS — allow Angular frontend ─────────────────────────────────────────
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",   // Angular dev
                "http://localhost:4201")   // Angular alt port
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ── Ocelot + SwaggerForOcelot ─────────────────────────────────────────────
builder.Services.AddOcelot(builder.Configuration).AddPolly();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// ── Middleware pipeline ───────────────────────────────────────────────────
app.UseCors("AllowAngular");

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
    opt.ReConfigureUpstreamSwaggerJson =
        AlterUpstreamSwaggerJson;
});

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

// Must be last — Ocelot handles all routing
await app.UseOcelot();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// ── Helper to prefix all swagger paths with gateway routes ────────────────
static string AlterUpstreamSwaggerJson(
    HttpContext context, string swaggerJson) => swaggerJson;