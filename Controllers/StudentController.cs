using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELearningSystem.Services;
using ELearningSystem.DTOs;
using System.Security.Claims;

namespace ELearningSystem.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        private int GetStudentId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentId = GetStudentId();
            var courses = await _studentService.GetStudentCourses(studentId);
            return Ok(courses);
        }

        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourseDetails(int courseId)
        {
            var studentId = GetStudentId();
            var course = await _studentService.GetCourseDetails(courseId, studentId);
            if (course == null)
                return NotFound();
            return Ok(course);
        }

        [HttpGet("courses/{courseId}/quizzes")]
        public async Task<IActionResult> GetCourseQuizzes(int courseId)
        {
            var studentId = GetStudentId();
            var quizzes = await _studentService.GetCourseQuizzes(courseId, studentId);
            return Ok(quizzes);
        }

        [HttpPost("submissions")]
        public async Task<IActionResult> SubmitAnswer([FromForm] SubmissionDto submissionDto)
        {
            try
            {
                var studentId = GetStudentId();
                var submission = await _studentService.SubmitAnswer(submissionDto, studentId);
                return Ok(submission);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("quizzes/{quizId}/score")]
        public async Task<IActionResult> GetQuizScore(int quizId)
        {
            var studentId = GetStudentId();
            var score = await _studentService.GetQuizScore(quizId, studentId);
            if (score == null)
                return NotFound();
            return Ok(score);
        }
    }
}