using Microsoft.AspNetCore.Mvc;
using ELearningSystem.Services;
using ELearningSystem.DTOs;

namespace ELearningSystem.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			try
			{
				var result = await _authService.Login(loginDto);

				if (result == null)
				{
					return Unauthorized(new { message = "Invalid username or password" });
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}