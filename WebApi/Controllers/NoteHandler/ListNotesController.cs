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
        public async Task<ActionResult<NoteListResponse>> Handle()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var notes = await _connection.QueryAsync<Note>(
                "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId",
                new { UserId = int.Parse(userIdClaim.Value) }
            );

            return Ok(new NoteListResponse(notes.Select(note => new NoteResponse(note))));
        }

        public record NoteResponse(int Id, string Title, string? Content, DateTime CreatedAt)
        {
            public NoteResponse(Note note) : this(note.Id, note.Title, note.Content, note.CreatedAt) { }
        };

        public record NoteListResponse(IEnumerable<NoteResponse> Notes);
    }
}
