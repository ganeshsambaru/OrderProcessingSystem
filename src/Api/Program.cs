using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessingSystem.Data;
using OrderProcessingSystem.Services.Dtos;
using OrderProcessingSystem.Services.Implementations;
using OrderProcessingSystem.Services.Implementations.Repositories;
using OrderProcessingSystem.Services.Interfaces;
using OrderProcessingSystem.Services.Interfaces.Repositories;
using OrderProcessingSystem.Validators;
using System.IO;

var builder = FunctionsApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure Application Insights
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register logging and dependencies
builder.Services.AddLogging();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateDtoValidator>();

builder.Services.AddScoped<IValidator<OrderCreateDto>, OrderCreateDtoValidator>();
builder.Services.AddScoped<IValidator<OrderUpdateDto>, OrderUpdateDtoValidator>();

// ✅ Register DbContext with SQL Server connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     sqlOptions => sqlOptions.MigrationsAssembly("Services")));

builder.Build().Run();
