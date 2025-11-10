using System.IO;
using System.Threading.Tasks;

namespace Infra.Storage
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
        Task<Stream?> GetFileAsync(string filePath);
    }
}
