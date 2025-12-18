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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var notes = await _connection.QueryAsync<NoteRequest>(
                "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId",
                new { UserId = int.Parse(userIdClaim.Value) }
            );

            return Ok(notes);
        }

        public record NoteRequest(int Id, string Title, string Content, DateTime CreatedAt, DateTime UpdatedAt);
    }
}
