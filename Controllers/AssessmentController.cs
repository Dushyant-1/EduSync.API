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
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetCourseAssessments(int courseId)
        {
            try
            {
                var assessments = await _assessmentService.GetAssessmentsByCourseAsync(courseId);
                return Ok(assessments);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDto>> GetAssessmentById(int id)
        {
            try
            {
                var assessment = await _assessmentService.GetAssessmentByIdAsync(id);
                return Ok(assessment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<AssessmentDto>> CreateAssessment([FromBody] CreateAssessmentDto createAssessmentDto)
        {
            try
            {
                var assessment = await _assessmentService.CreateAssessmentAsync(createAssessmentDto);
                return CreatedAtAction(nameof(GetAssessmentById), new { id = assessment.Id }, assessment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<AssessmentDto>> UpdateAssessment(int id, [FromBody] UpdateAssessmentDto updateAssessmentDto)
        {
            try
            {
                var assessment = await _assessmentService.UpdateAssessmentAsync(id, updateAssessmentDto);
                return Ok(assessment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult> DeleteAssessment(int id)
        {
            try
            {
                await _assessmentService.DeleteAssessmentAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/publish")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> PublishAssessment(int id)
        {
            try
            {
                var assessment = await _assessmentService.GetAssessmentByIdAsync(id);
                if (assessment == null)
                    return NotFound("Assessment not found");

                await _assessmentService.PublishAssessmentAsync(id);
                return Ok(new { message = "Assessment published successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 