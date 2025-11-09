using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Motorcycle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Year { get; set; }


        [Required]
        [MaxLength(200)]
        public string Model { get; set; }

        [Required]
        [MaxLength(10)]
        public string Plate { get; set; }

        public ICollection<Rental> Rentals { get; set; }

    }
}