using System.Data;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers.NoteHandler
{
    [Route("api/notes")]
    [ApiController]
    public class CreateNoteController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public CreateNoteController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPost(Name = "CreateNote")]
        [Authorize]
        public async Task<IActionResult> Handle([FromBody] CreateNoteRequest request)
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

            var sql = @"
                INSERT INTO Notes (Title, Content, CreatedAt, UpdatedAt, UserId)
                VALUES (@Title, @Content, @CreatedAt, @UpdatedAt, @UserId);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var newNote = new Note
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = int.Parse(userIdClaim.Value)
            };

            var newId = await _connection.ExecuteScalarAsync<int>(sql, newNote);
            newNote.Id = newId;

            return CreatedAtAction(nameof(Handle), new { id = newId }, new { note = new CreateNoteResponse(newNote)});
        }

        public record CreateNoteRequest(string Title, string Content);
        public record CreateNoteResponse(int Id, string Title, string? Content)
        {
            public CreateNoteResponse(Note note) : this(note.Id, note.Title, note.Content) { }
        };
    }
}