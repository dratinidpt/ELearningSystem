using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ELearningSystem.Services;
using ELearningSystem.DTOs;

namespace ELearningSystem.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ========== STUDENTS ==========
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _adminService.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _adminService.GetStudentById(id);
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDto studentDto)
        {
            try
            {
                var student = await _adminService.CreateStudent(studentDto);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto studentDto)
        {
            try
            {
                var student = await _adminService.UpdateStudent(id, studentDto);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _adminService.DeleteStudent(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========== TEACHERS ==========
        [HttpGet("teachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            var teachers = await _adminService.GetAllTeachers();
            return Ok(teachers);
        }

        [HttpGet("teachers/{id}")]
        public async Task<IActionResult> GetTeacher(int id)
        {
            var teacher = await _adminService.GetTeacherById(id);
            if (teacher == null)
                return NotFound();
            return Ok(teacher);
        }

        [HttpPost("teachers")]
        public async Task<IActionResult> CreateTeacher([FromBody] TeacherDto teacherDto)
        {
            try
            {
                var teacher = await _adminService.CreateTeacher(teacherDto);
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("teachers/{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDto teacherDto)
        {
            try
            {
                var teacher = await _adminService.UpdateTeacher(id, teacherDto);
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("teachers/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                await _adminService.DeleteTeacher(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========== COURSES ==========
        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _adminService.GetAllCourses();
            return Ok(courses);
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _adminService.GetCourseById(id);
            if (course == null)
                return NotFound();
            return Ok(course);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto courseDto)
        {
            try
            {
                var course = await _adminService.CreateCourse(courseDto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto courseDto)
        {
            try
            {
                var course = await _adminService.UpdateCourse(id, courseDto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _adminService.DeleteCourse(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}