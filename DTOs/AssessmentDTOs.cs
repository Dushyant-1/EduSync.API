using System.ComponentModel.DataAnnotations;

namespace EduSync.API.DTOs
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalMarks { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    public class CreateAssessmentDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal TotalMarks { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
    }

    public class UpdateAssessmentDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal TotalMarks { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
    }

    public class SubmitAssessmentDto
    {
        public List<SubmitAnswerDto> Answers { get; set; } = new List<SubmitAnswerDto>();
    }

    public class ResultDto
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal MarksObtained { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AssessmentTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
} 