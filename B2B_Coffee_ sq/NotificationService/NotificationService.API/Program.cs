using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Services;
using B2B.Common.Middlewares;
using Serilog;

// ── Configure Serilog ─────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/notification-service-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting NotificationService...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<NotificationDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("NotificationDb")));

builder.Services.AddScoped<INotificationDbContext, NotificationDbContext>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ── MassTransit — register ALL consumers ─────────────────────────────────
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<UserApprovedConsumer>();
    x.AddConsumer<OrderPlacedConsumer>();
    x.AddConsumer<OrderConfirmedConsumer>();
    x.AddConsumer<OrderStatusChangedConsumer>();
    x.AddConsumer<OrderRejectedConsumer>();
    x.AddConsumer<DeliveryStatusChangedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NotificationService — Ember&Bean",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// ── Auto-migrate ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<NotificationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService v1");
    c.RoutePrefix = string.Empty;
});

app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "NotificationService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}