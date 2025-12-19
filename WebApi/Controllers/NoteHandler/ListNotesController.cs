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
            var sort = HttpContext.Request.Query["sort"].ToString();

            string sql = "SELECT Id, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE UserId = @UserId";
            object queryParams = new { UserId = userId };

            if (!string.IsNullOrEmpty(q))
            {
                sql += " AND Title LIKE @SearchQuery";
                queryParams = new { UserId = userId, SearchQuery = $"%{q}%" };
            }

            switch (sort)
            {
                case "createdAtDesc":
                    sql += " ORDER BY CreatedAt DESC";
                    break;
                case "createdAtAsc":
                    sql += " ORDER BY CreatedAt ASC";
                    break;
                case "titleAsc":
                    sql += " ORDER BY Title ASC";
                    break;
                case "titleDesc":
                    sql += " ORDER BY Title DESC";
                    break;
                default:
                    sql += " ORDER BY Id DESC";
                    break;
            }
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
