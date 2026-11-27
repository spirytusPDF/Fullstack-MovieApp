using Domain;
using Services.DTOs.Comments;
using Repositories.CommentsRepo;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Services
{
    public enum CommentDeleteOutcome
    {
        Deleted,
        NotFound,
        Forbidden,
        Failed
    }

    public class CommentService(ICommentsRepository commentRepository, IMapper _mapper)
    {
        private readonly ICommentsRepository _commentRepository = commentRepository;
        public async Task<Result> AddCommentAsync(CommentCreateDTO commentCreateDTO)
        {
            var comment = _mapper.Map<MovieComment>(commentCreateDTO);
            comment.Posted = DateTime.Now;
            await _commentRepository.AddCommentAsync(comment);
            var res = await _commentRepository.SaveChangesAsync();
            if(res) return new Result() { IsSucces=true };
            else return new Result() {IsSucces=false, Message="Couldn't add a comment" };
        }
        public async Task<CommentDeleteOutcome> DeleteComment(int id, Guid userId, bool canDeleteAny)
        {
            var comment = await _commentRepository.GetOneAsync(id);
            if (comment == null) return CommentDeleteOutcome.NotFound;
            if (!canDeleteAny && comment.UserId != userId) return CommentDeleteOutcome.Forbidden;

            await _commentRepository.DeleteComment(comment);
            var res = await _commentRepository.SaveChangesAsync();
            return res ? CommentDeleteOutcome.Deleted : CommentDeleteOutcome.Failed;
        }


    }
}
