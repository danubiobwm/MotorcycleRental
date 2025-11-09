using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}