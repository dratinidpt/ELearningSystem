using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ELearningSystem.Data;
using ELearningSystem.DTOs;

namespace ELearningSystem.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            object user = null;
            int userId = 0;
            string fullName = "";

            switch (loginDto.UserType.ToLower())
            {
                case "admin":
                    var admin = await _context.Admins
                        .FirstOrDefaultAsync(a => a.Username == loginDto.Username);
                    if (admin != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.PasswordHash))
                    {
                        userId = admin.AdminId;
                        fullName = $"{admin.FirstName} {admin.LastName}";
                        user = admin;
                    }
                    break;

                case "teacher":
                    var teacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.Username == loginDto.Username);
                    if (teacher != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, teacher.PasswordHash))
                    {
                        userId = teacher.TeacherId;
                        fullName = $"{teacher.FirstName} {teacher.LastName}";
                        user = teacher;
                    }
                    break;

                case "student":
                    var student = await _context.Students
                        .FirstOrDefaultAsync(s => s.Username == loginDto.Username);
                    if (student != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, student.PasswordHash))
                    {
                        userId = student.StudentId;
                        fullName = $"{student.FirstName} {student.LastName}";
                        user = student;
                    }
                    break;
            }

            if (user == null)
                return null;

            var token = GenerateJwtToken(userId, loginDto.UserType);

            return new LoginResponseDto
            {
                Token = token,
                UserType = loginDto.UserType.ToLower(),
                UserId = userId,
                UserName = fullName
            };
        }

        private string GenerateJwtToken(int userId, string userType)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}