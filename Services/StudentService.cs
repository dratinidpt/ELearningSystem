using Microsoft.EntityFrameworkCore;
using ELearningSystem.Data;
using ELearningSystem.Models;
using ELearningSystem.DTOs;

namespace ELearningSystem.Services
{
    public interface IStudentService
    {
        Task<List<CourseResponseDto>> GetStudentCourses(int studentId);
        Task<CourseResponseDto> GetCourseDetails(int courseId, int studentId);
        Task<List<QuizResponseDto>> GetCourseQuizzes(int courseId, int studentId);
        Task<SubmissionResponseDto> SubmitAnswer(SubmissionDto dto, int studentId);
        Task<ScoreResponseDto> GetQuizScore(int quizId, int studentId);
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<CourseResponseDto>> GetStudentCourses(int studentId)
        {
            return await _context.CourseEnrollments
                .Where(ce => ce.StudentId == studentId)
                .Include(ce => ce.Course)
                .ThenInclude(c => c.Teacher)
                .Select(ce => new CourseResponseDto
                {
                    CourseId = ce.Course.CourseId,
                    CourseCode = ce.Course.CourseCode,
                    CourseName = ce.Course.CourseName,
                    Description = ce.Course.Description,
                    TeacherName = ce.Course.Teacher != null
                        ? $"{ce.Course.Teacher.FirstName} {ce.Course.Teacher.LastName}"
                        : "Not Assigned"
                }).ToListAsync();
        }

        public async Task<CourseResponseDto> GetCourseDetails(int courseId, int studentId)
        {
            var enrollment = await _context.CourseEnrollments
                .Include(ce => ce.Course)
                .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(ce => ce.CourseId == courseId && ce.StudentId == studentId);

            if (enrollment == null) return null;

            return new CourseResponseDto
            {
                CourseId = enrollment.Course.CourseId,
                CourseCode = enrollment.Course.CourseCode,
                CourseName = enrollment.Course.CourseName,
                Description = enrollment.Course.Description,
                TeacherName = enrollment.Course.Teacher != null
                    ? $"{enrollment.Course.Teacher.FirstName} {enrollment.Course.Teacher.LastName}"
                    : "Not Assigned"
            };
        }

        public async Task<List<QuizResponseDto>> GetCourseQuizzes(int courseId, int studentId)
        {
            // Verify student is enrolled
            var isEnrolled = await _context.CourseEnrollments
                .AnyAsync(ce => ce.CourseId == courseId && ce.StudentId == studentId);

            if (!isEnrolled)
                throw new Exception("You are not enrolled in this course");

            var quizzes = await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .Select(q => new
                {
                    Quiz = q,
                    Submission = q.Submissions.FirstOrDefault(s => s.StudentId == studentId)
                }).ToListAsync();

            return quizzes.Select(q => new QuizResponseDto
            {
                QuizId = q.Quiz.QuizId,
                Title = q.Quiz.Title,
                Description = q.Quiz.Description,
                FilePath = q.Quiz.FilePath,
                DueDate = q.Quiz.DueDate,
                TotalPoints = q.Quiz.TotalPoints,
                SubmittedDate = q.Submission?.SubmittedDate,
                Score = q.Submission?.Score
            }).ToList();
        }

        public async Task<SubmissionResponseDto> SubmitAnswer(SubmissionDto dto, int studentId)
        {
            // Verify student is enrolled in the course
            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .ThenInclude(c => c.CourseEnrollments)
                .FirstOrDefaultAsync(q => q.QuizId == dto.QuizId);

            if (quiz == null)
                throw new Exception("Quiz not found");

            var isEnrolled = quiz.Course.CourseEnrollments.Any(ce => ce.StudentId == studentId);
            if (!isEnrolled)
                throw new Exception("You are not enrolled in this course");

            // Check if already submitted
            var existingSubmission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.QuizId == dto.QuizId && s.StudentId == studentId);

            if (existingSubmission != null)
                throw new Exception("You have already submitted an answer for this quiz");

            // Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "submissions");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var submission = new Submission
            {
                QuizId = dto.QuizId,
                StudentId = studentId,
                FilePath = $"uploads/submissions/{uniqueFileName}",
                SubmittedDate = DateTime.Now
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return new SubmissionResponseDto
            {
                SubmissionId = submission.SubmissionId,
                FilePath = submission.FilePath,
                SubmittedDate = submission.SubmittedDate
            };
        }

        public async Task<ScoreResponseDto> GetQuizScore(int quizId, int studentId)
        {
            var submission = await _context.Submissions
                .Include(s => s.Quiz)
                .FirstOrDefaultAsync(s => s.QuizId == quizId && s.StudentId == studentId);

            if (submission == null || submission.Score == null)
                return null;

            return new ScoreResponseDto
            {
                QuizTitle = submission.Quiz.Title,
                TotalPoints = submission.Quiz.TotalPoints,
                Score = submission.Score.Value,
                SubmittedDate = submission.SubmittedDate,
                Feedback = submission.Feedback
            };
        }
    }
}