using System.ComponentModel.DataAnnotations;

namespace EduSync.API.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public decimal Marks { get; set; }
    }

    public class CreateQuestionDto
    {
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
        public string CorrectAnswer { get; set; } = string.Empty;

        public decimal Marks { get; set; }
    }

    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; } = string.Empty; // A, B, C, or D
    }
} 