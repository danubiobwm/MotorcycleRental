using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public record CourierCreateDto(Guid Id, string Name, string Cnpj, DateTime BirthDate, string CnhNumber, string CnhCategory);
}