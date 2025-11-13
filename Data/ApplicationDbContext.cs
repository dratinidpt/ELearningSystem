using Microsoft.EntityFrameworkCore;
using ELearningSystem.Models;

namespace ELearningSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CourseEnrollment>()
                .HasKey(ce => new { ce.CourseId, ce.StudentId });

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(ce => ce.Course)
                .WithMany(c => c.CourseEnrollments)
                .HasForeignKey(ce => ce.CourseId);

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(ce => ce.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(ce => ce.StudentId);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Quiz)
                .WithMany(q => q.Submissions)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ** ADD THIS BLOCK TO FIX THE WARNINGS **
            // Configure decimal precision to avoid data truncation
            modelBuilder.Entity<Quiz>()
                .Property(q => q.TotalPoints)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Submission>()
                .Property(s => s.Score)
                .HasColumnType("decimal(18, 2)");
            // ** END OF FIX **

            // Seed default admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@elearning.com",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}