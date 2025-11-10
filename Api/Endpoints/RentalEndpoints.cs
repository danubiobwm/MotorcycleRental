using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Entities;
using Application.Dtos;

namespace Api.Endpoints
{
    public static class RentalEndpoints
    {
        public static void MapRentalEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/rentals");

            // POST /rentals - criar locação
            group.MapPost("", async (RentalCreateDto dto, IRentalRepository rentalRepo, ICourierRepository courierRepo, IMotorcycleRepository motorcycleRepo) =>
            {
                if (dto == null)
                    return Results.BadRequest(new { message = "Request body is required" });

                var courier = await courierRepo.GetByIdAsync(dto.CourierId);
                if (courier == null)
                    return Results.BadRequest(new { message = "Courier not found" });

                // CNH eligibility (courier deve ter A ou A+B)
                var cnhCategoryProp = courier.GetType().GetProperty("CnhCategory");
                var cnh = cnhCategoryProp?.GetValue(courier)?.ToString()?.ToUpperInvariant() ?? "";
                if (!(cnh == "A" || cnh == "A+B"))
                    return Results.BadRequest(new { message = "Courier CNH not eligible for renting (needs A or A+B)" });

                var motorcycle = await motorcycleRepo.GetByIdAsync(dto.MotorcycleId);
                if (motorcycle == null)
                    return Results.BadRequest(new { message = "Motorcycle not found" });

                // plano e taxa diária
                if (!RentalPlan.TryGetPlan(dto.PlanDays, out var dailyRate))
                    return Results.BadRequest(new { message = "Invalid plan days" });

                var creationDate = DateTime.UtcNow;
                var startDate = creationDate.Date.AddDays(1);
                var expectedEndDate = startDate.AddDays(dto.PlanDays - 1);

                var rental = new Rental
                {
                    Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                    CourierId = dto.CourierId,
                    MotorcycleId = dto.MotorcycleId,
                    StartDate = startDate,
                    ExpectedEndDate = expectedEndDate,
                    PlanDays = dto.PlanDays,
                    DailyRate = dailyRate
                };

                // define CreationDate se existir no modelo
                var creationProp = rental.GetType().GetProperty("CreationDate");
                creationProp?.SetValue(rental, creationDate);

                await rentalRepo.AddAsync(rental);
                await rentalRepo.SaveChangesAsync();

                return Results.Created($"/rentals/{rental.Id}", new RentalDto(rental));
            })
            .Produces<RentalDto>(201);

            // PATCH /rentals/{id}/return - registrar devolução e calcular custo
            group.MapPatch("/{id:guid}/return", async (Guid id, RentalReturnDto dto, IRentalRepository rentalRepo) =>
            {
                if (dto == null)
                    return Results.BadRequest(new { message = "Request body is required" });

                var rental = await rentalRepo.GetByIdAsync(id);
                if (rental == null)
                    return Results.NotFound(new { message = "Rental not found" });

                var endDate = dto.ReturnDate.ToUniversalTime();

                // atualiza EndDate
                var endDateProp = rental.GetType().GetProperty("EndDate");
                endDateProp?.SetValue(rental, endDate);

                // calcula custo total
                var total = CalculateReturnCost(rental, endDate);

                // atualiza no banco
                var updateMethod = rentalRepo.GetType().GetMethod("UpdateAsync") ?? rentalRepo.GetType().GetMethod("Update");
                if (updateMethod != null)
                {
                    var result = updateMethod.Invoke(rentalRepo, new object[] { rental });
                    if (result is Task t) await t;
                }

                await rentalRepo.SaveChangesAsync();

                return Results.Ok(new ReturnResultDto(rental.Id, total, rental.StartDate, rental.ExpectedEndDate, endDate));
            })
            .Produces<ReturnResultDto>(200);

            // GET /rentals/{id} - buscar locação por ID
            group.MapGet("/{id:guid}", async (Guid id, IRentalRepository rentalRepo) =>
            {
                var r = await rentalRepo.GetByIdAsync(id);
                if (r == null)
                    return Results.NotFound(new { message = "Rental not found" });

                return Results.Ok(new RentalDto(r));
            })
            .Produces<RentalDto>(200);
        }

        // cálculo de custo
        private static decimal CalculateReturnCost(Rental r, DateTime endDate)
        {
            var start = r.StartDate.Date;
            var expected = r.ExpectedEndDate.Date;
            var actual = endDate.Date;

            decimal baseTotal = r.DailyRate * r.PlanDays;

            // devolução no prazo
            if (actual == expected)
                return baseTotal;

            // devolução antecipada
            if (actual < expected)
            {
                int unusedDays = (expected - actual).Days;
                int usedDays = r.PlanDays - unusedDays;
                decimal usedTotal = r.DailyRate * usedDays;

                decimal penaltyMultiplier = r.PlanDays switch
                {
                    7 => 0.20m,
                    15 => 0.40m,
                    _ => 0.0m
                };

                decimal penalty = r.DailyRate * unusedDays * penaltyMultiplier;
                return Math.Round(usedTotal + penalty, 2);
            }

            // devolução após o prazo
            int extraDays = (actual - expected).Days;
            decimal extras = extraDays * 50m;
            return Math.Round(baseTotal + extras, 2);
        }
    }

    // DTOs locais
    public record RentalCreateDto(Guid Id, Guid CourierId, Guid MotorcycleId, int PlanDays);

    public class RentalReturnDto
    {
        public DateTime ReturnDate { get; set; }
    }

    public record ReturnResultDto(Guid RentalId, decimal TotalCost, DateTime StartDate, DateTime ExpectedEndDate, DateTime? EndDate);

    // planos e tarifas
    public static class RentalPlan
    {
        public static bool TryGetPlan(int days, out decimal daily)
        {
            switch (days)
            {
                case 7: daily = 30m; return true;
                case 15: daily = 28m; return true;
                case 30: daily = 22m; return true;
                case 45: daily = 20m; return true;
                case 50: daily = 18m; return true;
                default: daily = 0m; return false;
            }
        }
    }
}
