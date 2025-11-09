using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


 namespace Application.Dtos
{
    public record MotorcycleDto(Guid Id, int Year, string Model, string Plate);
}