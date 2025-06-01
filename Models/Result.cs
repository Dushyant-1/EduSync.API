using System.ComponentModel.DataAnnotations;

namespace EduSync.API.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; } = null!;

        public decimal MarksObtained { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
        public DateTime? GradedAt { get; set; }
        public string Status { get; set; } = "Submitted"; // Submitted, Graded, Late
    }
} 