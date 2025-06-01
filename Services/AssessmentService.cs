using Microsoft.EntityFrameworkCore;
using EduSync.API.Data;
using EduSync.API.DTOs;
using EduSync.API.Models;
using EduSync.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EduSync.API.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureBlobStorageService _blobStorageService;

        public AssessmentService(ApplicationDbContext context, IAzureBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<IEnumerable<AssessmentDto>> GetAssessmentsByCourseAsync(int courseId)
        {
            return await _context.Assessments
                .Include(a => a.Questions)
                .Where(a => a.CourseId == courseId)
                .Select(a => new AssessmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    CourseId = a.CourseId,
                    DueDate = a.DueDate,
                    TotalMarks = a.TotalMarks,
                    Type = a.Type,
                    IsPublished = a.IsPublished,
                    Questions = a.Questions.Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        OptionA = q.OptionA,
                        OptionB = q.OptionB,
                        OptionC = q.OptionC,
                        OptionD = q.OptionD,
                        Marks = q.Marks
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<AssessmentDto> GetAssessmentByIdAsync(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            return new AssessmentDto
            {
                Id = assessment.Id,
                Title = assessment.Title,
                Description = assessment.Description,
                CourseId = assessment.CourseId,
                DueDate = assessment.DueDate,
                TotalMarks = assessment.TotalMarks,
                Type = assessment.Type,
                IsPublished = assessment.IsPublished,
                Questions = assessment.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    Marks = q.Marks
                }).ToList()
            };
        }

        public async Task<AssessmentDto> CreateAssessmentAsync(CreateAssessmentDto createAssessmentDto)
        {
            var course = await _context.Courses.FindAsync(createAssessmentDto.CourseId);
            if (course == null)
                throw new InvalidOperationException("Course not found");

            var assessment = new Assessment
            {
                Title = createAssessmentDto.Title,
                Description = createAssessmentDto.Description,
                CourseId = createAssessmentDto.CourseId,
                DueDate = createAssessmentDto.DueDate,
                TotalMarks = createAssessmentDto.TotalMarks,
                Type = createAssessmentDto.Type,
                CreatedAt = DateTime.UtcNow,
                IsPublished = true,
                Questions = createAssessmentDto.Questions.Select(q => new Question
                {
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectAnswer = q.CorrectAnswer,
                    Marks = q.Marks
                }).ToList()
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            return await GetAssessmentByIdAsync(assessment.Id);
        }

        public async Task<AssessmentDto> UpdateAssessmentAsync(int id, UpdateAssessmentDto updateAssessmentDto)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            assessment.Title = updateAssessmentDto.Title;
            assessment.Description = updateAssessmentDto.Description;
            assessment.DueDate = updateAssessmentDto.DueDate;
            assessment.TotalMarks = updateAssessmentDto.TotalMarks;
            assessment.Type = updateAssessmentDto.Type;
            assessment.UpdatedAt = DateTime.UtcNow;

            // Remove existing questions
            _context.Questions.RemoveRange(assessment.Questions);

            // Add new questions
            assessment.Questions = updateAssessmentDto.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Marks = q.Marks
            }).ToList();

            await _context.SaveChangesAsync();

            return await GetAssessmentByIdAsync(assessment.Id);
        }

        public async Task DeleteAssessmentAsync(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task<ResultDto> SubmitAssessmentAsync(int assessmentId, int studentId, SubmitAssessmentDto submitAssessmentDto)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == assessmentId);

            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            if (!assessment.IsPublished)
                throw new InvalidOperationException("Assessment is not published yet");

            if (DateTime.UtcNow > assessment.DueDate)
                throw new InvalidOperationException("Assessment submission deadline has passed");

            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
                throw new InvalidOperationException("Student not found");

            // Calculate marks
            decimal totalMarks = 0;
            foreach (var answer in submitAssessmentDto.Answers)
            {
                var question = assessment.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                    throw new InvalidOperationException($"Question with ID {answer.QuestionId} not found");

                if (answer.SelectedAnswer.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    totalMarks += question.Marks;
                }
            }

            var result = new Result
            {
                AssessmentId = assessmentId,
                StudentId = studentId,
                SubmissionDate = DateTime.UtcNow,
                MarksObtained = totalMarks,
                Status = "Graded" // Since it's MCQ, we can grade immediately
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return new ResultDto
            {
                Id = result.Id,
                AssessmentId = result.AssessmentId,
                StudentId = result.StudentId,
                SubmissionDate = result.SubmissionDate,
                MarksObtained = result.MarksObtained,
                Status = result.Status
            };
        }

        public async Task<ResultDto> GradeSubmissionAsync(int resultId, decimal marks)
        {
            var result = await _context.Results.FindAsync(resultId);
            if (result == null)
                throw new InvalidOperationException("Result not found");

            var assessment = await _context.Assessments.FindAsync(result.AssessmentId);
            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            if (marks > assessment.TotalMarks)
                throw new InvalidOperationException("Marks cannot exceed total marks");

            result.MarksObtained = marks;
            result.Status = "Graded";
            result.GradedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResultDto
            {
                Id = result.Id,
                AssessmentId = result.AssessmentId,
                StudentId = result.StudentId,
                SubmissionDate = result.SubmissionDate,
                MarksObtained = result.MarksObtained,
                Status = result.Status
            };
        }

        public async Task<IEnumerable<ResultDto>> GetAssessmentResultsAsync(int assessmentId)
        {
            return await _context.Results
                .Where(r => r.AssessmentId == assessmentId)
                .Select(r => new ResultDto
                {
                    Id = r.Id,
                    AssessmentId = r.AssessmentId,
                    StudentId = r.StudentId,
                    SubmissionDate = r.SubmissionDate,
                    MarksObtained = r.MarksObtained,
                    Status = r.Status
                })
                .ToListAsync();
        }

        public async Task<AssessmentDto> PublishAssessmentAsync(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                throw new InvalidOperationException("Assessment not found");

            assessment.IsPublished = true;
            await _context.SaveChangesAsync();

            return await GetAssessmentByIdAsync(id);
        }
    }
} 