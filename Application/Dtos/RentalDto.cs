using System;

namespace Application.Dtos
{
    public record RentalDto(
        Guid Id,
        Guid CourierId,
        Guid MotorcycleId,
        DateTime StartDate,
        DateTime ExpectedEndDate,
        DateTime RealStartDate,
        DateTime? EndDate,
        int PlanDays,
        decimal DailyRate
    );

    public record RentalReturnDto(Guid RentalId, DateTime ReturnDate);

    public record ReturnResultDto(Guid RentalId, decimal TotalCost, DateTime StartDate, DateTime ExpectedEndDate, DateTime? EndDate);
}
