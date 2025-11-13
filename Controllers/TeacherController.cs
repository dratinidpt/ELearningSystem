using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELearningSystem.Services;
using ELearningSystem.DTOs;
using System.Security.Claims;

namespace ELearningSystem.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        private int GetTeacherId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var teacherId = GetTeacherId();
            var courses = await _teacherService.GetTeacherCourses(teacherId);
            return Ok(courses);
        }

        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourseDetails(int courseId)
        {
            var teacherId = GetTeacherId();
            var course = await _teacherService.GetCourseDetails(courseId, teacherId);
            if (course == null)
                return NotFound();
            return Ok(course);
        }

        [HttpPost("quizzes")]
        public async Task<IActionResult> CreateQuiz([FromForm] QuizDto quizDto)
        {
            try
            {
                var teacherId = GetTeacherId();
                var quiz = await _teacherService.CreateQuiz(quizDto, teacherId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("courses/{courseId}/quizzes")]
        public async Task<IActionResult> GetCourseQuizzes(int courseId)
        {
            var teacherId = GetTeacherId();
            var quizzes = await _teacherService.GetCourseQuizzes(courseId, teacherId);
            return Ok(quizzes);
        }

        [HttpGet("quizzes/{quizId}/submissions")]
        public async Task<IActionResult> GetQuizSubmissions(int quizId)
        {
            var teacherId = GetTeacherId();
            var submissions = await _teacherService.GetQuizSubmissions(quizId, teacherId);
            return Ok(submissions);
        }

        [HttpPut("submissions/{submissionId}/score")]
        public async Task<IActionResult> ScoreSubmission(int submissionId, [FromBody] ScoreDto scoreDto)
        {
            try
            {
                var teacherId = GetTeacherId();
                await _teacherService.ScoreSubmission(submissionId, scoreDto, teacherId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}