using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public record RentalDto(Guid Id, Guid MotorcycleId, Guid CourierId, DateTime CreatedAt, DateTime StartDate, DateTime ExpectedEndDate, DateTime? EndDate, int PlanDays, decimal DailyRate);
}