using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteModel = WebApi.Models.Note;

namespace WebApi.Controllers.Note
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
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
        {
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                validationErrors["title"] = "Title is required.";
            }

            if(validationErrors.Count > 0)
            {
                return BadRequest(new { Title = "Validation Error", Errors = validationErrors });
            }

            var sql = @"
                INSERT INTO Notes (Title, Content, CreatedAt, UpdatedAt)
                VALUES (@Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var newNote = new NoteModel
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newId = await _connection.ExecuteScalarAsync<int>(sql, newNote);
            newNote.Id = newId;

            return CreatedAtAction(nameof(Create), new { id = newId }, newNote);
        }

        public record CreateNoteRequest(string Title, string Content);
    }
}
