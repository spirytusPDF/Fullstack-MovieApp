using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.CommentsRepo
{
    public interface ICommentsRepository
    {
        public List<MovieComment> GetByMovie(int movieId);
        public List<MovieComment> GetByUser(int userId);
        public MovieComment GetOne(int id);

        public bool AddComment(MovieComment comment);
        public bool DeleteComment(MovieComment comment);

    }
}
