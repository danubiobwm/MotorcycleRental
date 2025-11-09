using System;

namespace Application.Dtos
{
    public record MotorcycleCreateDto(Guid Id, int Year, string Model, string Plate);
}
