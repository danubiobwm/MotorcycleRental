using System;

namespace Application.Dtos
{
    public record RentalCreateDto(Guid MotorcycleId, Guid CourierId, int PlanDays, decimal DailyRate);
}
