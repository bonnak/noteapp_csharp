using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
