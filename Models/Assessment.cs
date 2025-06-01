using System.ComponentModel.DataAnnotations;

namespace EduSync.API.Models
{
    public class Assessment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime DueDate { get; set; }
        public decimal TotalMarks { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // Quiz, Assignment, Exam, etc.

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
} 