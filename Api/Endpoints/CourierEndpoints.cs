using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Storage;
using System.IO;
using Application.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace Api.Endpoints
{
    public static class CourierEndpoints
    {
        public static void MapCourierEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/couriers");

            // POST /couriers
            group.MapPost("", async (CourierCreateDto dto, ICourierRepository repo) =>
            {
                if (dto == null) return Results.BadRequest();

                // uniqueness checks
                var byCnpj = await repo.GetByCnpjAsync(dto.Cnpj);
                if (byCnpj != null) return Results.Conflict(new { message = "CNPJ already exists" });

                var byCnh = await repo.GetByCnhNumberAsync(dto.CnhNumber);
                if (byCnh != null) return Results.Conflict(new { message = "CNH number already exists" });

                if (!IsValidCnhCategory(dto.CnhCategory))
                    return Results.BadRequest(new { message = "Invalid CNH category" });

                var courier = new Courier
                {
                    Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                    Name = dto.Name,
                    Cnpj = dto.Cnpj,
                    BirthDate = dto.BirthDate,
                    CnhNumber = dto.CnhNumber,
                    CnhCategory = dto.CnhCategory,
                    CnhImagePath = dto.CnhImagePath
                };

                await repo.AddAsync(courier);
                await repo.SaveChangesAsync();

                return Results.Created($"/couriers/{courier.Id}", new CourierDto(courier));
            })
            .Produces<CourierDto>(201);

            // GET /couriers
            group.MapGet("", async (ICourierRepository repo) =>
            {
                var all = await repo.GetAllAsync();
                var list = all.Select(c => new CourierDto(c)).ToList();
                return Results.Ok(list);
            })
            .Produces<List<CourierDto>>(200);

            // GET /couriers/{id}
            group.MapGet("/{id:guid}", async (Guid id, ICourierRepository repo) =>
            {
                var c = await repo.GetByIdAsync(id);
                if (c == null) return Results.NotFound();
                return Results.Ok(new CourierDto(c));
            })
            .Produces<CourierDto>(200);

            // PUT /couriers/{id}/upload-cnh (multipart)
            group.MapPut("/{id:guid}/upload-cnh", async (Guid id, HttpRequest request, ICourierRepository repo, IFileStorage storage) =>
            {
                if (!request.HasFormContentType)
                    return Results.BadRequest("Expected form data");

                var form = await request.ReadFormAsync();
                var file = form.Files["file"];
                if (file == null)
                    return Results.BadRequest("No file provided in 'file' field");

                // validate mime
                if (file.ContentType != "image/png" && file.ContentType != "image/bmp")
                    return Results.BadRequest("Only PNG or BMP allowed");

                var courier = await repo.GetByIdAsync(id);
                if (courier == null) return Results.NotFound();

                var ext = Path.GetExtension(file.FileName);
                var filename = $"{Guid.NewGuid()}{ext}";
                await using var stream = file.OpenReadStream();

                var savedPath = await storage.SaveFileAsync(stream, filename, "cnh");
                courier.CnhImagePath = savedPath;

                await repo.UpdateAsync(courier);
                await repo.SaveChangesAsync();

                return Results.Ok(new { path = savedPath });
            })
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200);
        }

        private static bool IsValidCnhCategory(string category)
        {
            var upper = category?.ToUpperInvariant();
            return upper == "A" || upper == "B" || upper == "A+B";
        }
    }

    public record CourierCreateDto(Guid Id, string Name, string Cnpj, DateTime BirthDate, string CnhNumber, string CnhCategory, string? CnhImagePath);
    public record CourierDto(Guid Id, string Name, string Cnpj, DateTime BirthDate, string CnhNumber, string CnhCategory, string? CnhImagePath)
    {
        public CourierDto(Domain.Entities.Courier c)
            : this(c.Id, c.Name, c.Cnpj, c.BirthDate, c.CnhNumber, c.CnhCategory ?? string.Empty, c.CnhImagePath)
        { }
    }
}
