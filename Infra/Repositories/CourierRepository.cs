using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class CourierRepository : Repository<Courier>, ICourierRepository
    {
        public CourierRepository(AppDbContext context) : base(context) { }

        public async Task<Courier?> GetByCnpjAsync(string cnpj)
        {
            return await _context.Couriers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Cnpj.ToLower() == cnpj.ToLower());
        }

        public async Task<Courier?> GetByCnhNumberAsync(string cnhNumber)
        {
            return await _context.Couriers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CnhNumber.ToLower() == cnhNumber.ToLower());
        }
    }
}
