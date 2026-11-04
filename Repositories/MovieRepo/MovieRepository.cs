using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Contexts;
using System;
using System.Collections.Generic;
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

        public List<Movie> GetAll()
        {
            return _context.Movies
            .AsNoTracking()
            .Include(m => m.MovieComments)          
            .ThenInclude(c => c.user)          
            .ToList();
        }

        public Movie? GetOne(int id)
        {
            return _context.Movies
                .Include(m => m.MovieComments)
                .ThenInclude(c => c.user)
                .FirstOrDefault(m => m.Id == id);
        }

        public List<Movie> Search(string keyword)
        {
            return _context.Movies.Include(m => m.MovieComments).Where(m=>m.Title.ToLower().Contains(keyword) 
            || m.Genre.ToLower().Contains(keyword)).ToList();
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
