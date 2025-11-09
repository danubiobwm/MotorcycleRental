using System;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CourierService
    {
        private readonly ICourierRepository _courierRepository;

        public CourierService(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }

        public async Task<(bool Success, CourierDto? Data, string[] Errors)> CreateAsync(CourierCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cnpj))
                return (false, null, new[] { "CNPJ is required" });

            var exists = await _courierRepository.ExistsAsync(c => c.Cnpj == dto.Cnpj);
            if (exists)
                return (false, null, new[] { "CNPJ already registered" });

            var courier = new Courier
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Cnpj = dto.Cnpj,
                BirthDate = dto.BirthDate,
                CnhNumber = dto.CnhNumber,
                CnhCategory = dto.CnhCategory,
                CnhImagePath = dto.CnhImagePath
            };

            await _courierRepository.AddAsync(courier);

            var resultDto = new CourierDto(
                courier.Id,
                courier.Name,
                courier.Cnpj,
                courier.BirthDate,
                courier.CnhNumber,
                courier.CnhCategory ?? string.Empty,
                courier.CnhImagePath ?? string.Empty
            );

            return (true, resultDto, Array.Empty<string>());
        }
    }
}
