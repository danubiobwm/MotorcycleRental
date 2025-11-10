using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Application.Dtos;
using Infra.Messaging;
using System.Collections.Generic;

namespace Api.Endpoints
{
    public static class MotorcycleEndpoints
    {
        public static void MapMotorcycleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/motorcycles");

            // POST /motorcycles
            group.MapPost("", async (MotorcycleCreateDto dto, IMotorcycleRepository repo, IRabbitMqPublisher publisher) =>
            {
                if (dto == null) return Results.BadRequest();

                var existing = await repo.GetByPlateAsync(dto.Plate);
                if (existing != null)
                    return Results.Conflict(new { message = "Plate already exists" });

                var motorcycle = new Motorcycle
                {
                    Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                    Year = dto.Year,
                    Model = dto.Model,
                    Plate = dto.Plate
                };

                await repo.AddAsync(motorcycle);
                await repo.SaveChangesAsync();

                // publish MotorcycleRegistered event
                var @event = new
                {
                    Id = motorcycle.Id,
                    Year = motorcycle.Year,
                    Model = motorcycle.Model,
                    Plate = motorcycle.Plate
                };

                await publisher.PublishAsync("MotorcycleRegistered", @event);

                return Results.Created($"/motorcycles/{motorcycle.Id}", new MotorcycleDto(motorcycle));
            })
            .Produces<MotorcycleDto>(201)
            .WithName("CreateMotorcycle");

            // GET /motorcycles
            group.MapGet("", async (string? plate, IMotorcycleRepository repo) =>
            {
                if (!string.IsNullOrWhiteSpace(plate))
                {
                    var m = await repo.GetByPlateAsync(plate);
                    if (m == null) return Results.NotFound();
                    return Results.Ok(new List<MotorcycleDto> { new MotorcycleDto(m) });
                }

                var all = await repo.GetAllAsync();
                var list = all.Select(m => new MotorcycleDto(m));
                return Results.Ok(list);
            })
            .Produces<List<MotorcycleDto>>(200)
            .WithName("ListMotorcycles");

            // PUT /motorcycles/{id}/plate
            group.MapPut("/{id:guid}/plate", async (Guid id, PlateUpdateDto body, IMotorcycleRepository repo) =>
            {
                var m = await repo.GetByIdAsync(id);
                if (m == null) return Results.NotFound();

                var byPlate = await repo.GetByPlateAsync(body.Plate);
                if (byPlate != null && byPlate.Id != id) return Results.Conflict(new { message = "Plate already in use" });

                m.Plate = body.Plate;
                await repo.UpdateAsync(m);
                await repo.SaveChangesAsync();
                return Results.NoContent();
            })
            .Produces(204)
            .WithName("UpdateMotorcyclePlate");

            // DELETE /motorcycles/{id}
            group.MapDelete("/{id:guid}", async (Guid id, IMotorcycleRepository repo, IRentalRepository rentalRepo) =>
            {
                var m = await repo.GetByIdAsync(id);
                if (m == null) return Results.NotFound();

                var hasRentals = await rentalRepo.HasRentalsForMotorcycleAsync(id);
                if (hasRentals) return Results.BadRequest(new { message = "Cannot delete motorcycle with rentals" });

                await repo.DeleteAsync(m);
                await repo.SaveChangesAsync();
                return Results.NoContent();
            })
            .Produces(204)
            .WithName("DeleteMotorcycle");
        }
    }

    public record MotorcycleCreateDto(Guid Id, int Year, string Model, string Plate);
    public record MotorcycleDto(Guid Id, int Year, string Model, string Plate)
    {
        public MotorcycleDto(Motorcycle m) : this(m.Id, m.Year, m.Model, m.Plate) { }
    }

    public record PlateUpdateDto(string Plate);
}
