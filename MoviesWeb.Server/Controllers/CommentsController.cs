using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.Comments;
using System.Security.Claims;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(CommentService _commentService,
        IValidator<AddCommentContract> _addValidator,
        IMapper _mapper) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentContract contract, [FromQuery] int movieId, [FromQuery] string userId)
        {
            var valid = _addValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);

            var comment = _mapper.Map<CommentCreateDTO>(contract);
            if (Guid.TryParse(userId, out Guid result))
            {
                comment.UserId = result;
            }
            else return BadRequest(new { res = "User id is poorly written" });
             comment.MovieId = movieId;
            var res = await _commentService.AddCommentAsync(comment);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }
        [Authorize]
        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userId, out var parsedUserId)) return Unauthorized();

            var canDeleteAny = User.IsInRole("Manager") || User.IsInRole("Admin");
            var outcome = await _commentService.DeleteComment(id, parsedUserId, canDeleteAny);

            return outcome switch
            {
                CommentDeleteOutcome.Deleted => Ok(),
                CommentDeleteOutcome.NotFound => NotFound(),
                CommentDeleteOutcome.Forbidden => Forbid(),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Couldn't delete a comment")
            };
        }
    }
}
