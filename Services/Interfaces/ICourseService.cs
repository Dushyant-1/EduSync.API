using EduSync.API.DTOs;
using Microsoft.AspNetCore.Http;

namespace EduSync.API.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int instructorId);
        Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto);
        Task DeleteCourseAsync(int id);
        Task<string> UploadCourseMaterialAsync(int courseId, IFormFile file);
        Task DeleteCourseMaterialAsync(int courseId);
        Task<IEnumerable<CourseMaterialDto>> GetCourseMaterialsAsync(int courseId);
    }
} 