using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduSync.API.DTOs;
using EduSync.API.Services.Interfaces;

namespace EduSync.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IResultService _resultService;

        public ResultController(IAssessmentService assessmentService, IResultService resultService)
        {
            _assessmentService = assessmentService;
            _resultService = resultService;
        }

        [HttpPost("assessment/{assessmentId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ResultDto>> SubmitAssessment(int assessmentId, SubmitAssessmentDto submission)
        {
            try
            {
                var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _assessmentService.SubmitAssessmentAsync(assessmentId, studentId, submission);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{resultId}/grade")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<ResultDto>> GradeSubmission(int resultId, [FromBody] decimal marks)
        {
            try
            {
                var result = await _resultService.GradeSubmissionAsync(resultId, marks);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetAssessmentResults(int assessmentId)
        {
            try
            {
                var results = await _resultService.GetAssessmentResultsAsync(assessmentId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetStudentResults()
        {
            try
            {
                var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (studentId == 0)
                {
                    return Unauthorized("Student ID not found.");
                }
                var results = await _resultService.GetResultsByStudentAsync(studentId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            try
            {
                await _resultService.DeleteResultAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 