using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduSync.API.Data;
using EduSync.API.Models;
using Microsoft.EntityFrameworkCore;
using EduSync.API.DTOs;

namespace EduSync.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")] // Only students can enroll
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("course/{courseId}")]
        public async Task<ActionResult> EnrollStudent(int courseId)
        {
            // Get student ID from authenticated user
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                return Unauthorized("Student not found or not authenticated.");
            }

            // Check if the course exists
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Check if student is already enrolled
            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment)
            {
                return BadRequest("Student is already enrolled in this course.");
            }

            // Create new enrollment
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true,
                Status = "Active"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Enrollment successful.");
        }

        [HttpGet("student")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetEnrolledCourses()
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                return Unauthorized("Student not found or not authenticated.");
            }

            var enrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Title = e.Course.Title,
                    Description = e.Course.Description,
                    InstructorName = $"{e.Course.Instructor.FirstName} {e.Course.Instructor.LastName}",
                    CreatedAt = e.Course.CreatedAt,
                    IsActive = e.Course.IsActive,
                    Duration = e.Course.Duration,
                    Level = e.Course.Level
                })
                .ToListAsync();

            return Ok(enrolledCourses);
        }

         [HttpGet("course/{courseId}")]
        public async Task<ActionResult<bool>> IsStudentEnrolledInCourse(int courseId)
        {
             var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                // If student is not authenticated, they are not enrolled
                return Ok(false);
            }

            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            return Ok(isEnrolled);
        }

        [HttpDelete("course/{courseId}")]
        public async Task<ActionResult> UnenrollFromCourse(int courseId)
        {
            // Get student ID from authenticated user
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                return Unauthorized("Student not found or not authenticated.");
            }

            // Find the enrollment
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound("You are not enrolled in this course.");
            }

            // Remove the enrollment
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Successfully unenrolled from the course.");
        }
    }
} 