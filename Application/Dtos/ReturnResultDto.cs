using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public record ReturnResultDto(Guid RentalId, decimal Total, DateTime Start, DateTime ExpectedEnd, DateTime? End);
}