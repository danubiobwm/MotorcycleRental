using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMotorcycleRepository : IRepository<Motorcycle>
    {
        Task<Motorcycle?> GetByPlateAsync(string plate);
        Task<bool> HasRentalsAsync(Guid motorcycleId);
    }
}
