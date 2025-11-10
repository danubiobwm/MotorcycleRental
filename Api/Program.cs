using Application.Services;
using Domain.Interfaces;
using Infra.Data;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ======================
//  CONFIGURAÇÕES BASE
// ======================
builder.Services.AddControllers();

// Banco de Dados (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios e Serviços
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<CourierService>();
builder.Services.AddScoped<RentalService>();
builder.Services.AddScoped<MotorcycleService>();

// ======================
//  SWAGGER CONFIG
// ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Motorcycle Rental API",
        Version = "v1",
        Description = "API para gestão de motocicletas, entregadores e aluguéis."
    });
});

// ======================
//  APP
// ======================
var app = builder.Build();

// 🔧 Garante criação do banco (opcional, ajuda muito no primeiro run)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Habilita Swagger sempre (em DEV e PROD)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorcycle Rental API v1");
    c.RoutePrefix = string.Empty; // Abre Swagger direto em http://localhost:8080/
});

// Middlewares padrão
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
