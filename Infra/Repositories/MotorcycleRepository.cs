using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Repositories
{

    public class MotorcycleRepository : IRepository<Motorcycle>
    {
        private readonly AppDbContext _context;

        public MotorcycleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Motorcycle entity)
        {
            await _context.Motorcycles.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _context.Motorcycles
                .AsNoTracking()
                .OrderBy(x => x.Model)
                .ToListAsync();
        }


        public async Task<Motorcycle> GetByIdAsync(Guid id)
        {
            return await _context.Motorcycles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Motorcycle> GetByPlateAsync(string plate)
        {
            return await _context.Motorcycles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Plate.ToLower() == plate.ToLower());
        }

        public void Update(Motorcycle entity)
        {
            _context.Motorcycles.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(Motorcycle entity)
        {
            _context.Motorcycles.Remove(entity);
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<bool> PlateExistsAsync(string plate)
        {
            return await _context.Motorcycles.AnyAsync(x => x.Plate.ToLower() == plate.ToLower());
        }
    }
}
