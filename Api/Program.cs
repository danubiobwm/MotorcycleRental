using Application.Services;
using Domain.Interfaces;
using Infra.Data;
using Infra.Repositories;
using Infra.Messaging;
using Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Repositories ---
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

// --- Application Services ---
builder.Services.AddScoped<MotorcycleService>();
builder.Services.AddScoped<CourierService>();
builder.Services.AddScoped<RentalService>();

// --- Infra Services ---
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

// --- Background Consumer ---
builder.Services.AddHostedService<MotorcycleRegisteredConsumer>();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "🚀 Motorcycle Rental API Running!");

// --- Map Endpoints ---
app.MapMotorcycleEndpoints();
app.MapCourierEndpoints();
app.MapRentalEndpoints();

app.Run();
