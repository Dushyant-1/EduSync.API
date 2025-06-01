using System.Threading.Tasks;

namespace EduSync.API.Services.Interfaces
{
    public interface IAIModelService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> AnalyzeTextAsync(string text);
        Task<string> GenerateFeedbackAsync(string assessment, string studentResponse);
        Task<string> SummarizeTextAsync(string text);
    }
} 