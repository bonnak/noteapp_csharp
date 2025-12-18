using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
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
