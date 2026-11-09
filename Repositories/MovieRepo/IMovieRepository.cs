using Domain;
using Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories.MovieRepo
{
    public interface IMovieRepository
    {
        
        public Movie? GetOne(int id);
        public Tuple<List<Movie>, int> GetPagedMovies(Expression<Func<Movie, bool>> predicate, int page, int size, string sort);


        public bool AddMovie(Movie movie);
        public bool UpdateMovie(Movie movie);
        public bool DeleteMovie(Movie movie);

    }
}
