using AutoMapper;
using MoviesWeb.Server.Contracts;
using Services.DTOs.Movie;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class MovieMapperProfile: Profile
    {
        public MovieMapperProfile() 
        {
            CreateMap<AddMovieContract, MovieCreateDTO>().ReverseMap();
            CreateMap<UpdateMovieContract, UpdateMovieDTO>().ReverseMap();
        }
    }
}
