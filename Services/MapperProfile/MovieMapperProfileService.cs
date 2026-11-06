using AutoMapper;
using Domain;
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
        }
    }
}
