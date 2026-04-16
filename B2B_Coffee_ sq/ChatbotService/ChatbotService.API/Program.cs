using ChatbotService.Application.Commands;
using ChatbotService.Application.Interfaces;
using ChatbotService.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IGroqChatbotProvider, GroqChatbotProvider>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AskChatbotCommandHandler).Assembly));

// CORS for local development testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:5007"); // Assigning port 5007 for ChatbotService
