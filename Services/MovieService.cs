using AutoMapper;
using Domain;
using Repositories.MovieRepo;
using Services.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class MovieService(IMovieRepository movieRepository, IMapper _mapper)
    {
        private readonly IMovieRepository _movieRepository = movieRepository;

        public Result AddMovie(MovieCreateDTO movieCreateDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreateDTO);
            var res = _movieRepository.AddMovie(movie);
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't add a movie" };
        }

        public Result UpdateMovie(UpdateMovieDTO movieUpdateDTO)
        {
            var movie = _mapper.Map<Movie>(movieUpdateDTO);
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
            var movies = _movieRepository.GetAll();
            return _mapper.Map<List<MovieResponseDTO>>(movies);
        }
        public MovieResponseDTO? GetMovie(int id)
        {
            var movie = _movieRepository.GetOne(id);
            if (movie is null) return null;

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public List<MovieResponseDTO> SearchMovies(string keyWord)
        {
            var movies = _movieRepository.Search(keyWord);
            return _mapper.Map<List<MovieResponseDTO>>(movies);
        }
    }
}
