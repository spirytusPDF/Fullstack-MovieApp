using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.Comments;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(CommentService _commentService,
        IValidator<AddCommentContract> _addValidator) : ControllerBase
    {
        [HttpPost]
        public IActionResult AddComment(AddCommentContract contract, [FromQuery] int movieId, [FromQuery] int userId)
        {
            var valid = _addValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);

            var comment = new CommentCreateDTO()
            {
                Title = contract.Title,
                Text = contract.Text,
                UserId = userId,
                MovieId = movieId
            };
            var res = _commentService.AddComment(comment);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public IActionResult DeleteComment(int id)
        {
            var res = _commentService.DeleteComment(id);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }
    }
}
