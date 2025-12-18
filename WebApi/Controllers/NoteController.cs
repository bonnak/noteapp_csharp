using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public NoteController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet(Name = "GetNotes")]
        public IEnumerable<NoteDto> Get()
        {
            return Enumerable.Range(1, 5).Select(index =>
                new NoteDto
                (
                    Title: $"Note {index}",
                    Content: "This is a sample note content."
                ))
                .ToArray();
        }

        public record NoteDto(string Title, string Content)
        {
        }

        [HttpPost(Name = "CreateNote")]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
        {
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                validationErrors["Title"] = "Title is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                validationErrors["Content"] = "Content is required.";
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

            var newNote = new Note
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newId = await _connection.ExecuteScalarAsync<int>(sql, newNote);
            newNote.Id = newId;

            return CreatedAtAction(nameof(Get), new { id = newId }, newNote);
        }

        public record CreateNoteRequest(string Title, string Content);
    }
}
