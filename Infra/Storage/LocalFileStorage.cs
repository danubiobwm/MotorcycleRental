namespace Infra.Storage
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(Stream file, string fileName, string folder);
    }

    public class LocalFileStorage : IFileStorage
    {
        public async Task<string> SaveFileAsync(Stream file, string fileName, string folder)
        {
            var path = Path.Combine("/app/storage", folder);
            Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, fileName);

            using var output = File.Create(filePath);
            await file.CopyToAsync(output);
            return filePath;
        }
    }
}
