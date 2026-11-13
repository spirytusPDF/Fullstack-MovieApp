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
        
        public Task<Movie?> GetOneAsync(int id);
        public Task<Tuple<List<Movie>, int>>  GetPagedMoviesAsync(Expression<Func<Movie, bool>> predicate, int page, int size, string sort);


        public Task AddMovieAsync(Movie movie);
        public Task UpdateMovieAsync(Movie movie);
        public Task DeleteMovie(Movie movie);

        public Task<bool> SaveChangesAsync();

    }
}
