using Domain;
using Repositories.MovieRepo;
using Services.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class MovieService(IMovieRepository movieRepository)
    {
        private readonly IMovieRepository _movieRepository = movieRepository;

        public Result AddMovie(MovieCreateDTO movieCreateDTO)
        {
            var movie = new Movie()
            {
                Title = movieCreateDTO.Title,
                Description = movieCreateDTO.Description,
                Genre = movieCreateDTO.Genre,
                Rating = movieCreateDTO.Rating,
                ReleaseYear = movieCreateDTO.ReleaseYear,
                PosterUrl = movieCreateDTO.PosterUrl,
                MovieUrl = movieCreateDTO.MovieUrl,
            };
            var res = _movieRepository.AddMovie(movie);
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't add a movie" };
        }

        public Result UpdateMovie(UpdateMovieDTO movieUpdateDTO)
        {
            var movie = new Movie()
            {
                Id = movieUpdateDTO.Id,
                Description = movieUpdateDTO.Description,
                Genre = movieUpdateDTO.Genre,
                Rating = movieUpdateDTO.Rating,
                ReleaseYear = movieUpdateDTO.ReleaseYear,
                PosterUrl = movieUpdateDTO.PosterUrl,
                MovieUrl = movieUpdateDTO.MovieUrl,
            };
            var res = _movieRepository.UpdateMovie(movie);
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't update a movie" };
        }

        public Result DeleteMovie(int id)
        {
            var movie = _movieRepository.GetOne(id);
            if (movie == null) return new Result() { IsSucces = false, Message = "Couldn't find movie" };

            var res = _movieRepository.DeleteMovie(movie);
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't delete a movie" };
        }


        public List<MovieResponseDTO> GetMovies()
        {
            return _movieRepository.GetAll()
             .Select(m => new MovieResponseDTO()
             {
                 Id=m.Id,
                 Title = m.Title,
                 Description = m.Description,
                 Genre = m.Genre,
                 Rating = m.Rating,
                 ReleaseYear = m.ReleaseYear,
                 PosterUrl = m.PosterUrl,
                 MovieUrl = m.MovieUrl,
                 MovieComments = m.MovieComments
                 .Select(c=>new DTOs.Comments.CommentResponseDTO() 
                 { Id = c.Id, Title = c.Title,Text=c.Text, Posted = c.Posted, MovieName = m.Title, UserLogin = c.user.Login}).ToList()

             }).ToList();
        }
        public MovieResponseDTO? GetMovie(int id)
        {
            var movie = _movieRepository.GetOne(id);
            if (movie is null) return null;

            return new MovieResponseDTO()
            {
                Id= movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Rating = movie.Rating,
                ReleaseYear = movie.ReleaseYear,
                PosterUrl = movie.PosterUrl,
                MovieUrl = movie.MovieUrl,
                MovieComments = movie.MovieComments?
                    .Select(c => new DTOs.Comments.CommentResponseDTO()
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Text = c.Text,
                        Posted = c.Posted,
                        MovieName = movie.Title,
                        UserLogin = c.user.Login
                    })
                    .ToList() ?? []
            };
        }

        public List<MovieResponseDTO> SearchMovies(string keyWord)
        {
            return _movieRepository.Search(keyWord)
             .Select(m => new MovieResponseDTO()
             {
                 Title = m.Title,
                 Description = m.Description,
                 Genre = m.Genre,
                 Rating = m.Rating,
                 ReleaseYear = m.ReleaseYear,
                 MovieComments = m.MovieComments
                 .Select(c => new DTOs.Comments.CommentResponseDTO()
                 { Id = c.Id, Title = c.Title, Text = c.Text, Posted = c.Posted, MovieName = c.movie.Title, UserLogin = c.user.Login }).ToList()

             }).ToList();
        }
    }
}
