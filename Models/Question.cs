using System.ComponentModel.DataAnnotations;

namespace EduSync.API.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, or D

        public decimal Marks { get; set; }

        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; } = null!;
    }
} 