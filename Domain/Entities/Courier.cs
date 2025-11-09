using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Courier
    {
       [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    [Required, MaxLength(20)]
    public string Cnpj { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required, MaxLength(50)]
    public string CnhNumber { get; set; }

    [Required, MaxLength(10)]
    public string? CnhCategory { get; set; }

    public string? CnhImagePath { get; set; }
    }
}
