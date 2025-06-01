using System.ComponentModel.DataAnnotations;

namespace EduSync.API.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? InstructorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
    }

    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public int? Duration { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }

    public class CourseUploadDto
    {
        public IFormFile? File { get; set; }
        public string? Description { get; set; }
    }

    public class CourseMaterialDto
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
} 