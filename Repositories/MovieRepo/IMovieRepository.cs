using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.MovieRepo
{
    public interface IMovieRepository
    {
        public List<Movie> GetAll();
        public Movie? GetOne(int id);
        public List<Movie> Search(string keyword);


        public bool AddMovie(Movie movie);
        public bool UpdateMovie(Movie movie);
        public bool DeleteMovie(Movie movie);

    }
}
