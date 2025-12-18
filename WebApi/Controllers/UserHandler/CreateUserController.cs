using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers.UserHandler
{
    [Route("api/users")]
    [ApiController]
    public class CreateUserController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public CreateUserController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
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

            return CreatedAtAction(nameof(Create), new { id = newId }, newUser);
        }

        public record CreateUserRequest(string Username, string Password);
    }
}
