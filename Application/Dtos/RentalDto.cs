using System;

namespace Application.Dtos
{
    public record RentalDto(
    Guid Id,
    Guid CourierId,
    Guid MotorcycleId,
    DateTime StartDate,
    DateTime ExpectedEndDate,
    DateTime? EndDate,
    int PlanDays,
    decimal DailyRate)
    {
        public RentalDto(Domain.Entities.Rental r) : this(
            r.Id,
            r.CourierId,
            r.MotorcycleId,
            r.StartDate,
            r.ExpectedEndDate,
            r.EndDate,
            r.PlanDays,
            r.DailyRate)
        { }
    }
    public record RentalReturnDto(Guid RentalId, DateTime ReturnDate);

    public record ReturnResultDto(Guid RentalId, decimal TotalCost, DateTime StartDate, DateTime ExpectedEndDate, DateTime? EndDate);
}
