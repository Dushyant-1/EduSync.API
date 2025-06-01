using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EduSync.API.Data;
using EduSync.API.DTOs;
using EduSync.API.Models;
using EduSync.API.Services.Interfaces;

namespace EduSync.API.Services
{
    public class ResultService : IResultService
    {
        private readonly ApplicationDbContext _context;

        public ResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteResultAsync(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
                throw new InvalidOperationException("Result not found");

            _context.Results.Remove(result);
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
                Status = "Graded"
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

        public async Task<IEnumerable<ResultDto>> GetAssessmentResultsAsync(int assessmentId)
        {
            return await _context.Results
                .Where(r => r.AssessmentId == assessmentId)
                .Include(r => r.Student)
                .Select(r => new ResultDto
                {
                    Id = r.Id,
                    AssessmentId = r.AssessmentId,
                    StudentId = r.StudentId,
                    StudentName = r.Student.FirstName + " " + r.Student.LastName,
                    SubmissionDate = r.SubmissionDate,
                    MarksObtained = r.MarksObtained,
                    Status = r.Status
                })
                .ToListAsync();
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
            result.GradedAt = DateTime.UtcNow; // Assuming a GradedAt property exists

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

        // Add method to get results for a specific student
        public async Task<IEnumerable<ResultDto>> GetResultsByStudentAsync(int studentId)
        {
            return await _context.Results
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Assessment) // Include Assessment to get title
                .ThenInclude(a => a.Course) // Include Course to get title
                .Select(r => new ResultDto
                {
                    Id = r.Id,
                    AssessmentId = r.AssessmentId,
                    StudentId = r.StudentId,
                    SubmissionDate = r.SubmissionDate,
                    MarksObtained = r.MarksObtained,
                    Status = r.Status,
                    AssessmentTitle = r.Assessment.Title, // Map assessment title
                    CourseTitle = r.Assessment.Course.Title, // Map course title
                    TotalMarks = r.Assessment.TotalMarks // Include TotalMarks
                })
                .ToListAsync();
        }
    }
} 