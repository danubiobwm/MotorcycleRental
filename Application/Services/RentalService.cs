using System;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Repositories;

namespace Application.Services
{
    public class RentalService
    {
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IRepository<Courier> _courierRepository;
        private readonly IRepository<Motorcycle> _motorcycleRepository;

        public RentalService(
            IRepository<Rental> rentalRepository,
            IRepository<Courier> courierRepository,
            IRepository<Motorcycle> motorcycleRepository)
        {
            _rentalRepository = rentalRepository;
            _courierRepository = courierRepository;
            _motorcycleRepository = motorcycleRepository;
        }

        public async Task<RentalDto?> CreateRentalAsync(RentalCreateDto dto)
        {
            var courier = await _courierRepository.GetByIdAsync(dto.CourierId);
            var motorcycle = await _motorcycleRepository.GetByIdAsync(dto.MotorcycleId);


            if (courier == null || motorcycle == null)
                return null;

            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                Courier = courier,
                Motorcycle = motorcycle,
                StartDate = DateTime.UtcNow,
                ExpectedEndDate = DateTime.UtcNow.AddDays(dto.PlanDays),
                PlanDays = dto.PlanDays,
                DailyRate = dto.DailyRate
            };

            await _rentalRepository.AddAsync(rental);
            await _rentalRepository.SaveChangesAsync();

            return new RentalDto(
                rental.Id,
                courier.Id,
                motorcycle.Id,
                rental.StartDate,
                rental.ExpectedEndDate,
                rental.EndDate,
                rental.PlanDays,
                rental.DailyRate,
                rental.TotalCost
            );
        }

        public async Task<ReturnResultDto?> ReturnRentalAsync(RentalReturnDto dto)
        {
            var rental = await _rentalRepository.GetByIdAsync(dto.RentalId);
            if (rental == null)
                return null;

            rental.EndDate = dto.ReturnDate;
            var totalDays = (rental.EndDate.Value - rental.StartDate).Days;
            var totalCost = totalDays * rental.DailyRate;

            await _rentalRepository.UpdateAsync(rental);
            await _rentalRepository.SaveChangesAsync();

            return new ReturnResultDto(
                rental.Id,
                totalCost,
                rental.StartDate,
                rental.ExpectedEndDate,
                rental.EndDate
            );
        }
    }
}
