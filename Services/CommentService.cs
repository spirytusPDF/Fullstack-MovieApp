using Domain;
using Services.DTOs.Comments;
using Repositories.CommentsRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class CommentService(ICommentsRepository commentRepository)
    {
        private readonly ICommentsRepository _commentRepository = commentRepository;
        public Result AddComment(CommentCreateDTO commentCreateDTO)
        {
            var comment = new MovieComment()
            {
                Title = commentCreateDTO.Title,
                Text = commentCreateDTO.Text,
                UserId = commentCreateDTO.UserId,
                MovieId = commentCreateDTO.MovieId,
                Posted = DateTime.Now,

            };
            var res = _commentRepository.AddComment(comment);
            if(res) return new Result() { IsSucces=true };
            else return new Result() {IsSucces=false, Message="Couldn't add a comment" };
        }
        public Result DeleteComment(int id)
        {
            var comment = _commentRepository.GetOne(id);
            var res = _commentRepository.DeleteComment(comment);
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't delete a comment" };
        }


    }
}
