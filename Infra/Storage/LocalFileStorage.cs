using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infra.Storage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _basePath;
        public LocalFileStorage(IConfiguration configuration)
        {
            _basePath = configuration["Storage__Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
        {
            var folderPath = Path.Combine(_basePath, folder ?? "");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fs);

            // return relative path (folder/file)
            var relative = Path.Combine(folder, fileName).Replace("\\", "/");
            return relative;
        }

        public Task<Stream?> GetFileAsync(string filePath)
        {
            var full = Path.Combine(_basePath, filePath);
            if (!File.Exists(full)) return Task.FromResult<Stream?>(null);
            Stream s = new FileStream(full, FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream?>(s);
        }
    }
}
