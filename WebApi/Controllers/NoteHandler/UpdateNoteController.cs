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
    public class UpdateNoteController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public UpdateNoteController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPut(Name = "UpdateNote")]
        [Authorize]
        public async Task<ActionResult<UpdateNoteResponse>> Handle([FromBody] UpdateNoteRequest request)
        {
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                validationErrors["title"] = "Title is required.";
            }

            if (validationErrors.Count > 0)
            {
                return BadRequest(new { Title = "Validation Error", Errors = validationErrors });
            }
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdClaim.Value);

            var note = await _connection.QuerySingleOrDefaultAsync<Note>(
                "SELECT Id FROM Notes WHERE UserId = @UserId AND Id = @Id",
                new { 
                    UserId = userId,
                    Id = int.Parse(RouteData.Values["id"].ToString()) 
                }
            );

            if (note == null)
            {
                return NotFound();
            }

            var sql = @"
                UPDATE Notes 
                SET Title = @Title, 
                    Content = @Content, 
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId;
            ";

            var updatedNote = new Note
            {
                Id = note.Id,
                Title = request.Title,
                Content = request.Content,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            var rowAffected = await _connection.ExecuteAsync(sql, updatedNote);

            updatedNote = await _connection.QuerySingleOrDefaultAsync<Note>(
                "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE Id = @Id AND UserId = @UserId",
                new { 
                    Id = note.Id,
                    UserId = userId
                }
            );
            return Ok(new UpdateNoteResponseWrapper(new UpdateNoteResponse(updatedNote), rowAffected > 0));
        }

        public record UpdateNoteRequest(string Title, string Content);
        public record UpdateNoteResponse(int Id, string Title, string? Content)
        {
            public UpdateNoteResponse(Note note) : this(note.Id, note.Title, note.Content) { }
        };
        public record UpdateNoteResponseWrapper(UpdateNoteResponse Note, bool updated);
    }
}
