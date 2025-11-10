using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class MotorcycleService
    {
        private readonly IRepository<Motorcycle> _motorcycleRepository;

        public MotorcycleService(IRepository<Motorcycle> motorcycleRepository)
        {
            _motorcycleRepository = motorcycleRepository;
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _motorcycleRepository.GetAllAsync();
        }

        public async Task<(bool Success, Motorcycle? Data, string[] Errors)> CreateAsync(MotorcycleCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Plate))
                return (false, null, new[] { "Plate is required." });

            var motorcycle = new Motorcycle
            {
                Id = Guid.NewGuid(),
                Model = dto.Model,
                Plate = dto.Plate,
                Year = dto.Year
            };

            await _motorcycleRepository.AddAsync(motorcycle);
            await _motorcycleRepository.SaveChangesAsync();

            return (true, motorcycle, Array.Empty<string>());
        }
    }
}
