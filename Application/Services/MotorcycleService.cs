using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Retorna todas as motocicletas cadastradas.
        /// </summary>
        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _motorcycleRepository.GetAllAsync();
        }

        /// <summary>
        /// Retorna uma motocicleta pelo ID.
        /// </summary>
        public async Task<Motorcycle?> GetByIdAsync(Guid id)
        {
            return await _motorcycleRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria uma nova motocicleta com validações de duplicidade e dados obrigatórios.
        /// </summary>
        public async Task<(bool Success, Motorcycle? Data, string[] Errors)> CreateAsync(MotorcycleCreateDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Plate))
                errors.Add("Plate is required.");

            if (string.IsNullOrWhiteSpace(dto.Model))
                errors.Add("Model is required.");

            if (dto.Year < 2000 || dto.Year > DateTime.UtcNow.Year + 1)
                errors.Add("Year must be between 2000 and next year.");

            if (errors.Any())
                return (false, null, errors.ToArray());

            // Verificar duplicidade de placa
            var existing = (await _motorcycleRepository.GetAllAsync())
                .FirstOrDefault(m => m.Plate.Equals(dto.Plate, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return (false, null, new[] { $"Motorcycle with plate {dto.Plate} already exists." });

            var motorcycle = new Motorcycle
            {
                Id = Guid.NewGuid(),
                Plate = dto.Plate.ToUpperInvariant(),
                Model = dto.Model,
                Year = dto.Year
            };

            await _motorcycleRepository.AddAsync(motorcycle);
            await _motorcycleRepository.SaveChangesAsync();

            return (true, motorcycle, Array.Empty<string>());
        }
    }
}
