using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRentalRepository : IRepository<Rental>
    {
        Task<IEnumerable<Rental>> GetByCourierIdAsync(Guid courierId);
        Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId);
    }
}
