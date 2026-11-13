using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.CommentsRepo
{
    public interface ICommentsRepository
    {
        public Task<List<MovieComment>> GetByMovieAsync(int movieId);
        public Task<List<MovieComment>> GetByUserAsync(int userId);
        public Task<MovieComment?> GetOneAsync(int id);

        public Task AddCommentAsync(MovieComment comment);
        public Task DeleteComment(MovieComment comment);

        public Task<bool> SaveChangesAsync();

    }
}
