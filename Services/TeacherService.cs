using Microsoft.EntityFrameworkCore;
using ELearningSystem.Data;
using ELearningSystem.Models;
using ELearningSystem.DTOs;

namespace ELearningSystem.Services
{
    public interface ITeacherService
    {
        Task<List<CourseResponseDto>> GetTeacherCourses(int teacherId);
        Task<CourseResponseDto> GetCourseDetails(int courseId, int teacherId);
        Task<QuizResponseDto> CreateQuiz(QuizDto dto, int teacherId);
        Task<List<QuizResponseDto>> GetCourseQuizzes(int courseId, int teacherId);
        Task<QuizSubmissionsResponseDto> GetQuizSubmissions(int quizId, int teacherId);
        Task ScoreSubmission(int submissionId, ScoreDto dto, int teacherId);
    }

    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public TeacherService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<CourseResponseDto>> GetTeacherCourses(int teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.CourseEnrollments)
                .Select(c => new CourseResponseDto
                {
                    CourseId = c.CourseId,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    StudentsEnrolled = c.CourseEnrollments.Count
                }).ToListAsync();
        }

        public async Task<CourseResponseDto> GetCourseDetails(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Where(c => c.CourseId == courseId && c.TeacherId == teacherId)
                .Include(c => c.CourseEnrollments)
                .ThenInclude(ce => ce.Student)
                .FirstOrDefaultAsync();

            if (course == null) return null;

            return new CourseResponseDto
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Students = course.CourseEnrollments.Select(ce => new StudentResponseDto
                {
                    StudentId = ce.StudentId,
                    FirstName = ce.Student.FirstName,
                    LastName = ce.Student.LastName,
                    Email = ce.Student.Email
                }).ToList()
            };
        }

        public async Task<QuizResponseDto> CreateQuiz(QuizDto dto, int teacherId)
        {
            // Verify teacher owns the course
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == dto.CourseId && c.TeacherId == teacherId);

            if (course == null)
                throw new Exception("Course not found or you don't have permission");

            // Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "quizzes");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var quiz = new Quiz
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                FilePath = $"uploads/quizzes/{uniqueFileName}",
                DueDate = dto.DueDate,
                TotalPoints = dto.TotalPoints,
                CreatedDate = DateTime.Now
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return new QuizResponseDto
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                FilePath = quiz.FilePath,
                DueDate = quiz.DueDate,
                TotalPoints = quiz.TotalPoints
            };
        }

        public async Task<List<QuizResponseDto>> GetCourseQuizzes(int courseId, int teacherId)
        {
            // Verify teacher owns the course
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null)
                throw new Exception("Course not found or you don't have permission");

            return await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .Include(q => q.Submissions)
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Title = q.Title,
                    Description = q.Description,
                    FilePath = q.FilePath,
                    DueDate = q.DueDate,
                    TotalPoints = q.TotalPoints,
                    Submissions = q.Submissions.Count
                }).ToListAsync();
        }

        public async Task<QuizSubmissionsResponseDto> GetQuizSubmissions(int quizId, int teacherId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.Submissions)
                .ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null || quiz.Course.TeacherId != teacherId)
                throw new Exception("Quiz not found or you don't have permission");

            return new QuizSubmissionsResponseDto
            {
                QuizTitle = quiz.Title,
                TotalPoints = quiz.TotalPoints,
                Submissions = quiz.Submissions.Select(s => new SubmissionResponseDto
                {
                    SubmissionId = s.SubmissionId,
                    StudentName = $"{s.Student.FirstName} {s.Student.LastName}",
                    FilePath = s.FilePath,
                    SubmittedDate = s.SubmittedDate,
                    Score = s.Score,
                    Feedback = s.Feedback
                }).ToList()
            };
        }

        public async Task ScoreSubmission(int submissionId, ScoreDto dto, int teacherId)
        {
            var submission = await _context.Submissions
                .Include(s => s.Quiz)
                .ThenInclude(q => q.Course)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (submission == null || submission.Quiz.Course.TeacherId != teacherId)
                throw new Exception("Submission not found or you don't have permission");

            if (dto.Score > submission.Quiz.TotalPoints)
                throw new Exception("Score cannot exceed total points");

            submission.Score = dto.Score;
            submission.Feedback = dto.Feedback;
            submission.GradedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}