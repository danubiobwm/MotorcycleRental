using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Rental
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public Courier Courier { get; set; }

        [Required]
        public Guid MotorcycleId { get; set; }

        [ForeignKey(nameof(MotorcycleId))]
        public Motorcycle Motorcycle { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpectedEndDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int PlanDays { get; set; }

        public decimal DailyRate { get; set; }

        public decimal? TotalCost { get; set; }
    }
}
