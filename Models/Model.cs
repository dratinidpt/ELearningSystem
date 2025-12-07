using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningSystem.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Course> Courses { get; set; }
    }

    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; }
        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int? TeacherId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
    }

    public class CourseEnrollment
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrolledDate { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }

    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string FilePath { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalPoints { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }

    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }
        [Required]
        public int QuizId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public string FilePath { get; set; }
        public DateTime SubmittedDate { get; set; }
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime? GradedDate { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}