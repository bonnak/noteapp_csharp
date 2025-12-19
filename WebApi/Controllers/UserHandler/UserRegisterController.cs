using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers.UserHandler
{
    [Route("api/register")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public UserRegisterController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> Handle([FromBody] CreateUserRequest request)
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

            if(!string.IsNullOrWhiteSpace(request.Password) && request.ConfirmPassword != request.Password)
            {
                validationErrors["confirmPassword"] = new List<string> { "Passwords do not match." };
            }

            if(validationErrors.Count > 0)
            {
                return BadRequest(new { Message = "Validation Error", Errors = validationErrors });
            }

            var sql = @"
                INSERT INTO Users (Username, PasswordHash, CreatedAt, UpdatedAt)
                VALUES (@Username, @PasswordHash, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newId = await _connection.ExecuteScalarAsync<int>(sql, newUser);
            newUser.Id = newId;

            return CreatedAtAction(nameof(Handle), new { id = newId }, newUser);
        }

        public record CreateUserRequest(string Username, string Password, string ConfirmPassword);
    }
}
