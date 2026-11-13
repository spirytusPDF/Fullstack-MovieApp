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

        public async Task<Result> AddMovieAsync(MovieCreateDTO movieCreateDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreateDTO);
            await _movieRepository.AddMovieAsync(movie);
            var res = await _movieRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't add a movie" };
        }

        public async Task<Result> UpdateMovieAsync(UpdateMovieDTO movieUpdateDTO)
        {
            var movie = _mapper.Map<Movie>(movieUpdateDTO);
            await _movieRepository.UpdateMovieAsync(movie);
            var res = await _movieRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't update a movie" };
        }

        public async Task<Result> DeleteMovie(int id)
        {
            var movie = await _movieRepository.GetOneAsync(id);
            if (movie == null) return new Result() { IsSucces = false, Message = "Couldn't find movie" };
            await _movieRepository.DeleteMovie(movie);
            var res = await _movieRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't delete a movie" };
        }


        public async Task<MovieResponseDTO?> GetMovieAsync(int id)
        {
            var movie = await _movieRepository.GetOneAsync(id);
            if (movie is null) return null;

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<Tuple<List<MovieResponseDTO>, int>> GetMoviesAsync(MovieParamsDTO parameters)
        {
            
            var parametersOrg = _mapper.Map<MovieParameters>(parameters);
            if(parameters.Page == null || parameters.Page <= 0) parametersOrg.Page = 1;
            if (parameters.PageSize == null || parameters.PageSize <= 0) parametersOrg.PageSize = 10;
            if (string.IsNullOrEmpty(parametersOrg.Sort)) parametersOrg.Sort = string.Empty;

            if (parametersOrg.Key != null)
            {
                var moviesByKey = await _movieRepository.GetPagedMoviesAsync(x =>
                EF.Functions.Like(x.Title, $"%{parametersOrg.Key}%") ||
                EF.Functions.Like(x.Genre, $"%{parametersOrg.Key}%") ||
                EF.Functions.Like(x.Description, $"%{parametersOrg.Key}%"), 
                parametersOrg.Page, parametersOrg.PageSize, parametersOrg.Sort);

                return new(_mapper.Map<List<MovieResponseDTO>>(moviesByKey.Item1), moviesByKey.Item2);
            }

            var filter = await ExpressionFilterBuilder.BuildFilter<Movie, MovieParameters>(parametersOrg);
            var movies = await _movieRepository.GetPagedMoviesAsync(filter, parametersOrg.Page, parametersOrg.PageSize, parametersOrg.Sort);
            return new(_mapper.Map<List<MovieResponseDTO>>(movies.Item1), movies.Item2);
        }
    }
}
