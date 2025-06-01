using Microsoft.AspNetCore.Http;

namespace EduSync.API.Services.Interfaces
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath);
        Task<byte[]> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
        Task<IEnumerable<string>> ListFilesAsync(string folderPath);
        Task<string> GetFileUrlAsync(string fileName);
    }
} 