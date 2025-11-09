using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
