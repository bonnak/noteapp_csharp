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
    public class DeleteNoteController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public DeleteNoteController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpDelete(Name = "DeleteNote")]
        [Authorize]
        public async Task<ActionResult<DeleteNoteResponse>> Handle()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdClaim.Value);
            var noteId = int.Parse(RouteData.Values["id"].ToString());

            var sql = @"
                DELETE FROM Notes 
                WHERE Id = @Id AND UserId = @UserId;
            ";

            var rowAffected = await _connection.ExecuteAsync(sql, new {
                Id = noteId,
                UserId = userId
            });

            return Ok(new DeleteNoteResponse(rowAffected > 0));
        }

        public record DeleteNoteResponse(bool deleted);
    }
}
