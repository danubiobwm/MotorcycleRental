using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        public RentalRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Rental>> GetByCourierIdAsync(Guid courierId)
        {
            return await _context.Rentals.AsNoTracking()
                .Where(r => r.CourierId == courierId).ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId)
        {
            return await _context.Rentals.AsNoTracking()
                .Where(r => r.MotorcycleId == motorcycleId).ToListAsync();
        }
    }
}
