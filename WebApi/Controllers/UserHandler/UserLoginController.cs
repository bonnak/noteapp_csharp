using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers.UserHandler
{
    [Route("api/login")]
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
        public async Task<IActionResult> Handle([FromBody] LoginUserRequest request)
        {
            var validationErrors = new Dictionary<string, List<string>>();
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                validationErrors["username"] = new List<string> { "Username is required." };
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                validationErrors["password"] = new List<string> { "Password is required." };
            }

            if(validationErrors.Count > 0)
            {
                return BadRequest(new { Message = "Validation Error", Errors = validationErrors });
            }

            var user = await _connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username", 
                new { Username = request.Username }
            );
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest(new { Message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new { Message = "Login successful", Token = token });

        }

        public record LoginUserRequest(string Username, string Password);
    }
}
