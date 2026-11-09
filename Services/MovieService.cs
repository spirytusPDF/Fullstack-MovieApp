using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories.MovieRepo;
using Repositories.Queries;
using Services.DTOs.Movie;
using Services.Helpers;
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


        public MovieResponseDTO? GetMovie(int id)
        {
            var movie = _movieRepository.GetOne(id);
            if (movie is null) return null;

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public Tuple<List<MovieResponseDTO>, int> GetMovies(MovieParamsDTO parameters)
        {
            
            var parametersOrg = _mapper.Map<MovieParameters>(parameters);
            if(parameters.Page == null || parameters.Page <= 0) parametersOrg.Page = 1;
            if (parameters.PageSize == null || parameters.PageSize <= 0) parametersOrg.PageSize = 10;
            if (string.IsNullOrEmpty(parametersOrg.Sort)) parametersOrg.Sort = string.Empty;

            if (parametersOrg.Key != null)
            {
                
                var moviesByKey = _movieRepository.GetPagedMovies(x =>
                EF.Functions.Like(x.Title, $"%{parametersOrg.Key}%") ||
                EF.Functions.Like(x.Genre, $"%{parametersOrg.Key}%") ||
                EF.Functions.Like(x.Description, $"%{parametersOrg.Key}%"), 
                parametersOrg.Page, parametersOrg.PageSize, parametersOrg.Sort);

                return new(_mapper.Map<List<MovieResponseDTO>>(moviesByKey.Item1), moviesByKey.Item2);
            }

            var filter = ExpressionFilterBuilder.BuildFilter<Movie, MovieParameters>(parametersOrg);
            var movies = _movieRepository.GetPagedMovies(filter, parametersOrg.Page, parametersOrg.PageSize, parametersOrg.Sort);
            return new(_mapper.Map<List<MovieResponseDTO>>(movies.Item1), movies.Item2);
        }
    }
}
