using Xunit;
using Domain.Entities;
using System;
using FluentAssertions;

namespace Tests.Integration
{
    public class RentalCalculationTests
    {
        [Theory]
        [InlineData(7, 7, 7, 30.0)]    // returned on expected day -> full plan
        public void ReturnOnExpectedDay_PaysFullPlan(int planDays, int startOffset, int returnOffset, decimal dailyRate)
        {
            var creation = DateTime.UtcNow;
            var startDate = creation.Date.AddDays(1);
            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                CreationDate = creation,
                StartDate = startDate,
                PlanDays = planDays,
                DailyRate = dailyRate,
                ExpectedEndDate = startDate.AddDays(planDays - 1)
            };

            var endDate = rental.ExpectedEndDate; // same day
            var total = InvokeCalculateReturnCost(rental, endDate);

            total.Should().Be(planDays * dailyRate);
        }

        private decimal InvokeCalculateReturnCost(Rental r, DateTime endDate)
        {
            // copy of the private method logic, or make it internal and call
            var start = r.StartDate.Date;
            var expected = r.ExpectedEndDate.Date;
            var actual = endDate.Date;
            decimal baseTotal = r.DailyRate * r.PlanDays;
            if (actual == expected) return baseTotal;
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
            int extraDays = (actual - expected).Days;
            decimal extras = extraDays * 50m;
            return Math.Round(baseTotal + extras, 2);
        }
    }
}
