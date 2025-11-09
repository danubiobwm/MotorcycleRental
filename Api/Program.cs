using Application.Services;
using Domain.Interfaces;
using Infra.Data;
using Infra.Repositories;
using Infra.Messaging;
using Infra.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

// Add services
builder.Services.AddScoped<MotorcycleService>();
builder.Services.AddScoped<CourierService>();
builder.Services.AddScoped<RentalService>();

// Add background consumer
builder.Services.AddHostedService<MotorcycleRegisteredConsumer>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "🚀 Motorcycle Rental API Running!");
app.Run();
