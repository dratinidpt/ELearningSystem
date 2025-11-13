using Microsoft.EntityFrameworkCore;
using ELearningSystem.Data;
using ELearningSystem.Models;
using ELearningSystem.DTOs;

namespace ELearningSystem.Services
{
    public interface IAdminService
    {
        Task<List<StudentResponseDto>> GetAllStudents();
        Task<StudentResponseDto> GetStudentById(int id);
        Task<StudentResponseDto> CreateStudent(StudentDto dto);
        Task<StudentResponseDto> UpdateStudent(int id, StudentDto dto);
        Task DeleteStudent(int id);

        Task<List<TeacherResponseDto>> GetAllTeachers();
        Task<TeacherResponseDto> GetTeacherById(int id);
        Task<TeacherResponseDto> CreateTeacher(TeacherDto dto);
        Task<TeacherResponseDto> UpdateTeacher(int id, TeacherDto dto);
        Task DeleteTeacher(int id);

        Task<List<CourseResponseDto>> GetAllCourses();
        Task<CourseResponseDto> GetCourseById(int id);
        Task<CourseResponseDto> CreateCourse(CourseDto dto);
        Task<CourseResponseDto> UpdateCourse(int id, CourseDto dto);
        Task DeleteCourse(int id);
    }

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== STUDENTS ==========
        public async Task<List<StudentResponseDto>> GetAllStudents()
        {
            return await _context.Students
                .Select(s => new StudentResponseDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Username = s.Username
                }).ToListAsync();
        }

        public async Task<StudentResponseDto> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            return new StudentResponseDto
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Username = student.Username
            };
        }

        public async Task<StudentResponseDto> CreateStudent(StudentDto dto)
        {
            if (await _context.Students.AnyAsync(s => s.Username == dto.Username))
                throw new Exception("Username already exists");

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedDate = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Username = student.Username
            };
        }

        public async Task<StudentResponseDto> UpdateStudent(int id, StudentDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            if (await _context.Students.AnyAsync(s => s.Username == dto.Username && s.StudentId != id))
                throw new Exception("Username already exists");

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Email = dto.Email;
            student.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Username = student.Username
            };
        }

        public async Task DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        // ========== TEACHERS ==========
        public async Task<List<TeacherResponseDto>> GetAllTeachers()
        {
            return await _context.Teachers
                .Select(t => new TeacherResponseDto
                {
                    TeacherId = t.TeacherId,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    Email = t.Email,
                    Username = t.Username
                }).ToListAsync();
        }

        public async Task<TeacherResponseDto> GetTeacherById(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return null;

            return new TeacherResponseDto
            {
                TeacherId = teacher.TeacherId,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Username = teacher.Username
            };
        }

        public async Task<TeacherResponseDto> CreateTeacher(TeacherDto dto)
        {
            if (await _context.Teachers.AnyAsync(t => t.Username == dto.Username))
                throw new Exception("Username already exists");

            var teacher = new Teacher
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedDate = DateTime.Now
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return new TeacherResponseDto
            {
                TeacherId = teacher.TeacherId,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Username = teacher.Username
            };
        }

        public async Task<TeacherResponseDto> UpdateTeacher(int id, TeacherDto dto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                throw new Exception("Teacher not found");

            if (await _context.Teachers.AnyAsync(t => t.Username == dto.Username && t.TeacherId != id))
                throw new Exception("Username already exists");

            teacher.FirstName = dto.FirstName;
            teacher.LastName = dto.LastName;
            teacher.Email = dto.Email;
            teacher.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                teacher.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();

            return new TeacherResponseDto
            {
                TeacherId = teacher.TeacherId,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Username = teacher.Username
            };
        }

        public async Task DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                throw new Exception("Teacher not found");

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }

        // ========== COURSES ==========
        public async Task<List<CourseResponseDto>> GetAllCourses()
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.CourseEnrollments)
                .Select(c => new CourseResponseDto
                {
                    CourseId = c.CourseId,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    TeacherId = c.TeacherId,
                    TeacherName = c.Teacher != null ? $"{c.Teacher.FirstName} {c.Teacher.LastName}" : null,
                    StudentsEnrolled = c.CourseEnrollments.Count
                }).ToListAsync();
        }

        public async Task<CourseResponseDto> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.CourseEnrollments)
                .ThenInclude(ce => ce.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return null;

            return new CourseResponseDto
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                TeacherId = course.TeacherId,
                TeacherName = course.Teacher != null ? $"{course.Teacher.FirstName} {course.Teacher.LastName}" : null,
                StudentsEnrolled = course.CourseEnrollments.Count,
                EnrolledStudents = course.CourseEnrollments.Select(ce => new EnrolledStudentDto
                {
                    StudentId = ce.StudentId,
                    StudentName = $"{ce.Student.FirstName} {ce.Student.LastName}"
                }).ToList()
            };
        }

        public async Task<CourseResponseDto> CreateCourse(CourseDto dto)
        {
            if (await _context.Courses.AnyAsync(c => c.CourseCode == dto.CourseCode))
                throw new Exception("Course code already exists");

            var course = new Course
            {
                CourseCode = dto.CourseCode,
                CourseName = dto.CourseName,
                Description = dto.Description,
                TeacherId = dto.TeacherId,
                CreatedDate = DateTime.Now
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Enroll students
            if (dto.StudentIds != null && dto.StudentIds.Any())
            {
                foreach (var studentId in dto.StudentIds)
                {
                    var enrollment = new CourseEnrollment
                    {
                        CourseId = course.CourseId,
                        StudentId = studentId,
                        EnrolledDate = DateTime.Now
                    };
                    _context.CourseEnrollments.Add(enrollment);
                }
                await _context.SaveChangesAsync();
            }

            return await GetCourseById(course.CourseId);
        }

        public async Task<CourseResponseDto> UpdateCourse(int id, CourseDto dto)
        {
            var course = await _context.Courses
                .Include(c => c.CourseEnrollments)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                throw new Exception("Course not found");

            if (await _context.Courses.AnyAsync(c => c.CourseCode == dto.CourseCode && c.CourseId != id))
                throw new Exception("Course code already exists");

            course.CourseCode = dto.CourseCode;
            course.CourseName = dto.CourseName;
            course.Description = dto.Description;
            course.TeacherId = dto.TeacherId;

            // Update enrollments
            _context.CourseEnrollments.RemoveRange(course.CourseEnrollments);

            if (dto.StudentIds != null && dto.StudentIds.Any())
            {
                foreach (var studentId in dto.StudentIds)
                {
                    var enrollment = new CourseEnrollment
                    {
                        CourseId = course.CourseId,
                        StudentId = studentId,
                        EnrolledDate = DateTime.Now
                    };
                    _context.CourseEnrollments.Add(enrollment);
                }
            }

            await _context.SaveChangesAsync();
            return await GetCourseById(id);
        }

        public async Task DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                throw new Exception("Course not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}