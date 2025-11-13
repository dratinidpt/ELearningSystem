using Microsoft.AspNetCore.Http;

namespace ELearningSystem.DTOs
{
	public class LoginDto
	{
		public string UserType { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class LoginResponseDto
	{
		public string Token { get; set; }
		public string UserType { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
	}

	public class StudentDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class TeacherDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class CourseDto
	{
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public string Description { get; set; }
		public int? TeacherId { get; set; }
		public List<int> StudentIds { get; set; }
	}

	public class QuizDto
	{
		public int CourseId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime DueDate { get; set; }
		public decimal TotalPoints { get; set; }
		public IFormFile File { get; set; }
	}

	public class SubmissionDto
	{
		public int QuizId { get; set; }
		public IFormFile File { get; set; }
	}

	public class ScoreDto
	{
		public decimal Score { get; set; }
		public string Feedback { get; set; }
	}

	// Response DTOs
	public class StudentResponseDto
	{
		public int StudentId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
	}

	public class TeacherResponseDto
	{
		public int TeacherId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
	}

	public class CourseResponseDto
	{
		public int CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public string Description { get; set; }
		public int? TeacherId { get; set; }
		public string TeacherName { get; set; }
		public int StudentsEnrolled { get; set; }
		public List<StudentResponseDto> Students { get; set; }
		public List<EnrolledStudentDto> EnrolledStudents { get; set; }
	}

	public class EnrolledStudentDto
	{
		public int StudentId { get; set; }
		public string StudentName { get; set; }
	}

	public class QuizResponseDto
	{
		public int QuizId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string FilePath { get; set; }
		public DateTime DueDate { get; set; }
		public decimal TotalPoints { get; set; }
		public int Submissions { get; set; }
		public DateTime? SubmittedDate { get; set; }
		public decimal? Score { get; set; }
	}

	public class SubmissionResponseDto
	{
		public int SubmissionId { get; set; }
		public string StudentName { get; set; }
		public string FilePath { get; set; }
		public DateTime SubmittedDate { get; set; }
		public decimal? Score { get; set; }
		public string Feedback { get; set; }
	}

	public class QuizSubmissionsResponseDto
	{
		public string QuizTitle { get; set; }
		public decimal TotalPoints { get; set; }
		public List<SubmissionResponseDto> Submissions { get; set; }
	}

	public class ScoreResponseDto
	{
		public string QuizTitle { get; set; }
		public decimal TotalPoints { get; set; }
		public decimal Score { get; set; }
		public DateTime SubmittedDate { get; set; }
		public string Feedback { get; set; }
	}
}