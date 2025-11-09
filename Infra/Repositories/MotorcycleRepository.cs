using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class MotorcycleRepository : Repository<Motorcycle>, IMotorcycleRepository
    {
        public MotorcycleRepository(AppDbContext context) : base(context) { }

        public async Task<Motorcycle?> GetByPlateAsync(string plate)
        {
            return await _context.Motorcycles.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Plate.ToLower() == plate.ToLower());
        }

        public async Task<bool> HasRentalsAsync(Guid motorcycleId)
        {
            return await _context.Rentals.AnyAsync(r => r.MotorcycleId == motorcycleId);
        }
    }
}
