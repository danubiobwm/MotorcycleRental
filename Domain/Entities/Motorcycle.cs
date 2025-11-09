using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Motorcycle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [MaxLength(10)]
        public string Plate { get; set; }

        public int Year { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
