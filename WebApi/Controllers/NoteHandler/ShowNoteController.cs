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
    [Route("api/notes/{id}")]
    [ApiController]
    public class ShowNoteController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public ShowNoteController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet(Name = "GetNoteById")]
        [Authorize]
        public async Task<ActionResult<NoteResponse>> Handle()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var note = await _connection.QuerySingleOrDefaultAsync<Note>(
                "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId AND Id = @Id",
                new { 
                    UserId = int.Parse(userIdClaim.Value), 
                    Id = int.Parse(RouteData.Values["id"].ToString()) 
                }
            );

            if (note == null)
            {
                return NotFound(new NoteResponseWrapper(null));
            }


            return Ok(new NoteResponseWrapper(new NoteResponse(note)));
        }

        public record NoteResponse(int Id, string Title, string? Content)
        {
            public NoteResponse(Note note) : this(note.Id, note.Title, note.Content) { }
        };
        public record NoteResponseWrapper(NoteResponse Note);
    }
}
