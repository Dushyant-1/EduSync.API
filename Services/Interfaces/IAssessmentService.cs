using EduSync.API.DTOs;
using Microsoft.AspNetCore.Http;

namespace EduSync.API.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<IEnumerable<AssessmentDto>> GetAssessmentsByCourseAsync(int courseId);
        Task<AssessmentDto> GetAssessmentByIdAsync(int id);
        Task<AssessmentDto> CreateAssessmentAsync(CreateAssessmentDto createAssessmentDto);
        Task<AssessmentDto> UpdateAssessmentAsync(int id, UpdateAssessmentDto updateAssessmentDto);
        Task DeleteAssessmentAsync(int id);
        Task<ResultDto> SubmitAssessmentAsync(int assessmentId, int studentId, SubmitAssessmentDto submitAssessmentDto);
        Task<ResultDto> GradeSubmissionAsync(int resultId, decimal marks);
        Task<IEnumerable<ResultDto>> GetAssessmentResultsAsync(int assessmentId);
        Task<AssessmentDto> PublishAssessmentAsync(int id);
    }
} 