using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduSync.API.DTOs;

namespace EduSync.API.Services.Interfaces
{
    public interface IResultService
    {
        Task DeleteResultAsync(int id);
        Task<ResultDto> SubmitAssessmentAsync(int assessmentId, int studentId, SubmitAssessmentDto submitAssessmentDto);
        Task<ResultDto> GradeSubmissionAsync(int resultId, decimal marks);
        Task<IEnumerable<ResultDto>> GetAssessmentResultsAsync(int assessmentId);
        Task<IEnumerable<ResultDto>> GetResultsByStudentAsync(int studentId);
    }
} 