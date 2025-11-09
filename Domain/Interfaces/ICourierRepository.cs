using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICourierRepository : IRepository<Courier>
    {
        Task<Courier?> GetByCnpjAsync(string cnpj);
        Task<Courier?> GetByCnhNumberAsync(string cnhNumber);
    }
}
