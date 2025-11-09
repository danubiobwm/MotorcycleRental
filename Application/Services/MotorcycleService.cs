using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Repositories;

namespace Application.Services
{
    public class MotorcycleService
    {
        private readonly IRepository<Motorcycle> _repository;

        public MotorcycleService(IRepository<Motorcycle> repository)
        {
            _repository = repository;
        }

        public async Task<MotorcycleDto> CreateAsync(MotorcycleCreateDto dto)
        {
            var motorcycle = new Motorcycle
            {
                Id = Guid.NewGuid(),
                Year = dto.Year,
                Model = dto.Model,
                Plate = dto.Plate
            };

            await _repository.AddAsync(motorcycle);
            await _repository.SaveChangesAsync();

            return new MotorcycleDto(motorcycle.Id, motorcycle.Year, motorcycle.Model, motorcycle.Plate);
        }

        public async Task<IEnumerable<MotorcycleDto>> GetAllAsync()
        {
            var motorcycles = await _repository.GetAllAsync();

            return motorcycles.Select(m =>
                new MotorcycleDto(m.Id, m.Year, m.Model, m.Plate)
            );
        }

        public async Task<MotorcycleDto?> GetByIdAsync(Guid id)
        {
            var motorcycle = await _repository.GetByIdAsync(id);
            if (motorcycle == null)
                return null;

            return new MotorcycleDto(motorcycle.Id, motorcycle.Year, motorcycle.Model, motorcycle.Plate);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var motorcycle = await _repository.GetByIdAsync(id);
            if (motorcycle == null)
                return false;

            _repository.DeleteAsync(motorcycle);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
