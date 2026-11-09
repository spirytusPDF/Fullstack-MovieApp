using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Contexts;
using Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories.MovieRepo
{
    public class MovieRepository(AppDbContext context) : IMovieRepository
    {
        private readonly AppDbContext _context = context;
        public bool AddMovie(Movie movie)
        {
            try
            {
               _context.Movies.Add(movie);
               var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteMovie(Movie movie)
        {
            try
            {
                _context.Movies.Remove(movie);
                var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Movie? GetOne(int id)
        {
            return _context.Movies
                .Include(m => m.MovieComments)
                .ThenInclude(c => c.user)
                .FirstOrDefault(m => m.Id == id);
        }

        public Tuple<List<Movie>, int> GetPagedMovies(Expression<Func<Movie, bool>> predicate, int page, int size, string sort)
        {
            var query = _context.Movies.Where(predicate);
            var totalSize = query.Count();
            query = sort?.ToLowerInvariant() switch
            {
                "latest" => query.OrderByDescending(m => m.ReleaseYear).ThenBy(m => m.Id),
                "older" => query.OrderBy(m => m.ReleaseYear).ThenBy(m => m.Id),
                "rating" => query.OrderByDescending(m => m.Rating)
                                  .ThenByDescending(m => m.ReleaseYear)
                                  .ThenBy(m => m.Id),
                _ => query.OrderBy(m => m.Title).ThenBy(m => m.Id)
            };
            query = query.Skip((page - 1) * size).Take(size);
            return new (query.ToList(), totalSize);
        }

        public bool UpdateMovie(Movie movie)
        {
            try
            {
                var oldMovie = _context.Movies.FirstOrDefault(m => m.Id == movie.Id);
                if (oldMovie != null)
                {
                    oldMovie.Description = movie.Description;
                    oldMovie.Genre = movie.Genre;
                    oldMovie.ReleaseYear = movie.ReleaseYear;
                    oldMovie.Rating = movie.Rating;
                    oldMovie.PosterUrl = movie.PosterUrl;
                    oldMovie.MovieUrl = movie.MovieUrl;
                    var res = _context.SaveChanges();
                    return res > 0;
                }
                else return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
