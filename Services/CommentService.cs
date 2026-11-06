using Domain;
using Services.DTOs.Comments;
using Repositories.CommentsRepo;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Services
{
    public class CommentService(ICommentsRepository commentRepository, IMapper _mapper)
    {
        private readonly ICommentsRepository _commentRepository = commentRepository;
        public Result AddComment(CommentCreateDTO commentCreateDTO)
        {
            var comment = _mapper.Map<MovieComment>(commentCreateDTO);
            comment.Posted = DateTime.Now;
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
