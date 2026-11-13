using AutoMapper;
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
        IValidator<AddCommentContract> _addValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentContract contract, [FromQuery] int movieId, [FromQuery] int userId)
        {
            var valid = _addValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);

            var comment = _mapper.Map<CommentCreateDTO>(contract);
            comment.UserId = userId;
            comment.MovieId = movieId;
            var res = await _commentService.AddCommentAsync(comment);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var res = await _commentService.DeleteComment(id);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }
    }
}
