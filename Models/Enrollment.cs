using System.ComponentModel.DataAnnotations;

namespace EduSync.API.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public User? Student { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Active"; // Active, Completed, Dropped
    }
} 