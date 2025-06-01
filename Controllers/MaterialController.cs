using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduSync.API.Services.Interfaces;
using EduSync.API.DTOs;

namespace EduSync.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public MaterialController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("course/{courseId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<string>> UploadCourseMaterial(int courseId, IFormFile file)
        {
            try
            {
                var fileName = await _courseService.UploadCourseMaterialAsync(courseId, file);
                return Ok(new { fileName });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("course/{courseId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult> DeleteCourseMaterial(int courseId)
        {
            try
            {
                await _courseService.DeleteCourseMaterialAsync(courseId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<CourseMaterialDto>>> GetCourseMaterials(int courseId)
        {
            try
            {
                var materials = await _courseService.GetCourseMaterialsAsync(courseId);
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 