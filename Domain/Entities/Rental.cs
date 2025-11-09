using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Rental
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid MotorcycleId { get; set; }
        public Motorcycle Motorcycle { get; set; }


        [Required]
        public Guid CourierId { get; set; }
        public Courier Courier { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }


        [Required]
        public DateTime StartDate { get; set; }


        [Required]
        public DateTime ExpectedEndDate { get; set; }


        public DateTime? EndDate { get; set; }


        [Required]
        public int PlanDays { get; set; }


        [Required]
        public decimal DailyRate { get; set; }
    }
}