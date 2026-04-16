using AuthService.Application.Commands.RegisterClient;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using B2B.Common.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Text;

// ── Configure Serilog ─────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/auth-service-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting AuthService...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ── Database (EF Core 10) ────────────────────────────────────────────────
    builder.Services.AddDbContext<AuthDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

    // ── Dependency Injection ──────────────────────────────────────────────────
    builder.Services.AddScoped<IAuthDbContext>(sp => sp.GetRequiredService<AuthDbContext>());
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IOtpService, OtpService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    // ── MediatR ───────────────────────────────────────────────────────────────
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(RegisterClientCommandHandler).Assembly));

    // ── MassTransit ──────────────────────────────────────────────────────────
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
            });
            cfg.ConfigureEndpoints(ctx);
        });
    });

    // ── Authentication & Swagger ──────────────────────────────────────────────
    var jwt = builder.Configuration.GetSection("JwtSettings");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
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
                    Encoding.UTF8.GetBytes(jwt["Secret"] ?? "SuperSecretKeyForEmberAndBeanCoffee2026!"))
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService — Ember&Bean", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
        });
    });

    var app = builder.Build();

    // ── Global Correlation & Security Middleware ──────────────────────────
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    // ── Auto-Migration ────────────────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await db.Database.MigrateAsync();

        // ── Seed SuperAdmin (ensure one exists and has correct password) ──
        var superAdmin = await db.Users.FirstOrDefaultAsync(u => u.Role == AuthService.Domain.Enums.UserRole.SuperAdmin);
        if (superAdmin == null)
        {
            superAdmin = new AuthService.Domain.Entities.ApplicationUser
            {
                FullName = "Super Admin",
                Email = "superadmin@b2bcoffee.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "0000000000",
                CompanyName = "B2B Coffee Roastery",
                Role = AuthService.Domain.Enums.UserRole.SuperAdmin,
                Status = AuthService.Domain.Enums.UserStatus.Approved,
                IsEmailVerified = true,
                ApprovedAt = DateTime.UtcNow
            };
            db.Users.Add(superAdmin);
            await db.SaveChangesAsync();
            Log.Information("Seeded default SuperAdmin: superadmin@b2bcoffee.com");
        }
        else if (!superAdmin.PasswordHash.StartsWith("$2a$") ||
                 !BCrypt.Net.BCrypt.Verify("Admin@123", superAdmin.PasswordHash))
        {
            // Hash is corrupted or doesn't match the expected default password
            superAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            await db.SaveChangesAsync();
            Log.Information("Fixed corrupted SuperAdmin password hash.");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1"); c.RoutePrefix = string.Empty; });
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}