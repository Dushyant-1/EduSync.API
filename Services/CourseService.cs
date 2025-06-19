using Microsoft.EntityFrameworkCore;
using EduSync.API.Data;
using EduSync.API.DTOs;
using EduSync.API.Models;
using EduSync.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EduSync.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureBlobStorageService _blobStorageService;

        public CourseService(ApplicationDbContext context, IAzureBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Where(c => c.IsActive)
                .Include(c => c.Instructor)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    InstructorName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    Duration = c.Duration,
                    Level = c.Level
                })
                .ToListAsync();
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (course == null)
                throw new Exception("Course not found");

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int instructorId)
        {
            var instructor = await _context.Users.FindAsync(instructorId);
            if (instructor == null || instructor.Role != "Instructor")
                throw new Exception("Invalid instructor");

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                InstructorId = instructorId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorName = $"{instructor.FirstName} {instructor.LastName}",
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (course == null)
                throw new Exception("Course not found");

            course.Title = dto.Title ?? string.Empty;
            course.Description = dto.Description ?? string.Empty;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                throw new Exception("Course not found");

            course.IsActive = false;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadCourseMaterialAsync(int courseId, IFormFile file)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            return await _blobStorageService.UploadFileAsync(file, $"courses/{courseId}/materials");
        }

        public async Task DeleteCourseMaterialAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            var folderPath = $"courses/{courseId}/materials";
            var files = await _blobStorageService.ListFilesAsync(folderPath);
            
            foreach (var file in files)
            {
                if (file != folderPath) // Skip the folder itself
                {
                    await _blobStorageService.DeleteFileAsync(file);
                }
            }
        }

        public async Task<IEnumerable<CourseMaterialDto>> GetCourseMaterialsAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            var fileNames = await _blobStorageService.ListFilesAsync($"courses/{courseId}/materials");

            var materials = new List<CourseMaterialDto>();
            var folderPath = $"courses/{courseId}/materials";
            foreach (var fileName in fileNames)
            {
                if (fileName == folderPath) 
                {
                    continue;
                }
                var fileUrl = await _blobStorageService.GetFileUrlAsync(fileName);
                materials.Add(new CourseMaterialDto { FileName = fileName, FileUrl = fileUrl });
            }

            return materials;
        }
    }
} 