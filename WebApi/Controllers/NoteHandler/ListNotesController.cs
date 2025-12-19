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
            var userId = int.Parse(userIdClaim.Value);
            var q = HttpContext.Request.Query["q"].ToString();

            string sql = "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId";
            object queryParams = new { UserId = userId };

            if (!string.IsNullOrEmpty(q))
            {
                sql += " AND Title LIKE @SearchQuery";
                queryParams = new { UserId = userId, SearchQuery = $"%{q}%" };
            }
            sql += " ORDER BY Id DESC";
            var notes = await _connection.QueryAsync<Note>(sql,
                queryParams
            );

            return Ok(new NoteListResponse(notes.Select(note => new NoteResponse(note))));
        }

        public record NoteResponse(int Id, string Title, DateTime CreatedAt)
        {
            public NoteResponse(Note note) : this(note.Id, note.Title, note.CreatedAt) { }
        };

        public record NoteListResponse(IEnumerable<NoteResponse> Notes);
    }
}
