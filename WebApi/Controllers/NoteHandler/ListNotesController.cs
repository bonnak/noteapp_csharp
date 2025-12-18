using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using WebApi.Models;

namespace WebApi.Controllers.NoteHandler
{
    [Route("api/notes")]
    [ApiController]
    public class ListNotesController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public ListNotesController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet(Name = "GetNotes")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<NoteRequest>>> Handle()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim not found in token." });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(new { message = "Invalid User ID format in token." });
            }

            var notes = await _connection.QueryAsync<NoteRequest>(
                "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId",
                new { UserId = userId }
            );

            return Ok(notes);
        }

        public record NoteRequest(int Id, string Title, string Content, DateTime CreatedAt, DateTime UpdatedAt);
    }
}
