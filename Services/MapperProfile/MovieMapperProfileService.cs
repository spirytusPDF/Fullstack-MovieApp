using AutoMapper;
using Domain;
using Repositories.Queries;
using Services.DTOs.Movie;

namespace MoviesWeb.Server.MapperProfile
{
    public class MovieMapperProfileService: Profile
    {
        public MovieMapperProfileService() 
        {
            CreateMap<Movie, MovieCreateDTO>().ReverseMap();
            CreateMap<Movie, UpdateMovieDTO>().ReverseMap();
            CreateMap<Movie, MovieResponseDTO>().ReverseMap();
            CreateMap<MovieParameters, MovieParamsDTO>().ReverseMap();
        }
    }
}
