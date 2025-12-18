using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers.UserHandler
{
    [Route("api/users/login")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly IDbConnection _connection;
        private readonly JwtService _jwtService;

        public UserLoginController(IDbConnection connection, JwtService jwtService)
        {
            _connection = connection;
            _jwtService = jwtService;
        }

        [HttpPost(Name = "LoginUser")]
        public async Task<IActionResult> handle([FromBody] LoginUserRequest request)
        {
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                validationErrors["username"] = "Username is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                validationErrors["password"] = "Password is required.";
            }

            if(validationErrors.Count > 0)
            {
                return BadRequest(new { Title = "Validation Error", Errors = validationErrors });
            }

            var hashedPassword = await _connection.QuerySingleOrDefaultAsync<string>(
                "SELECT PasswordHash FROM Users WHERE Username = @Username", 
                new { Username = request.Username }
            );
            if (!BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(1, request.Username);
            
            return Ok(new { Message = "Login successful", Token = token });

        }

        public record LoginUserRequest(string Username, string Password);
    }
}
