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
        public async Task AddMovieAsync(Movie movie)
        {
            try
            {
               await _context.Movies.AddAsync(movie);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteMovie(Movie movie)
        {
            try
            {
                _context.Movies.Remove(movie);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<Movie?> GetOneAsync(int id)
        {
            return await _context.Movies.Include(m => m.MovieComments)
                .ThenInclude(c => c.user)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Tuple<List<Movie>, int>> GetPagedMoviesAsync(Expression<Func<Movie, bool>> predicate, int page, int size, string sort)
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
            return  new (await query.ToListAsync(), totalSize);
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            try
            {
                var oldMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movie.Id);
                if (oldMovie != null)
                {
                    oldMovie.Description = movie.Description;
                    oldMovie.Genre = movie.Genre;
                    oldMovie.ReleaseYear = movie.ReleaseYear;
                    oldMovie.Rating = movie.Rating;
                    oldMovie.PosterUrl = movie.PosterUrl;
                    oldMovie.MovieUrl = movie.MovieUrl;
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0 ? true : false;
        }
    }
}
