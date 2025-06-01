using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using EduSync.API.Services.Interfaces;

namespace EduSync.API.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var fileName = $"{folderPath}/{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return fileName;
        }

        public async Task<byte[]> DownloadFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            Console.WriteLine($"Attempting to download blob with name: {fileName}");
            Console.WriteLine($"Container name: {_containerName}");
            Console.WriteLine($"Full blob URI: {blobClient.Uri}");

            if (!await blobClient.ExistsAsync())
            {
                Console.WriteLine($"Blob does not exist: {fileName}");
                throw new Exception($"File not found: {fileName}");
            }

            Console.WriteLine($"Blob exists, proceeding with download");
            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            Console.WriteLine($"Download completed successfully");
            return memoryStream.ToArray();
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
            }
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string folderPath)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var files = new List<string>();

            await foreach (var blob in containerClient.GetBlobsAsync(prefix: folderPath))
            {
                files.Add(blob.Name);
            }

            return files;
        }

        public async Task<string> GetFileUrlAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
                throw new Exception("File not found");

            return blobClient.Uri.ToString();
        }
    }
} 