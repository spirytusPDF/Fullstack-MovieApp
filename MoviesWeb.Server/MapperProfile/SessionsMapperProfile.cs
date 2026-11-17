using AutoMapper;
using Domain;
using MoviesWeb.Server.Contracts;
using Repositories.Queries;
using Services.DTOs.MovieSession;

namespace MoviesWeb.Server.MapperProfile
{
    public class SessionsMapperProfile:Profile
    {
        public SessionsMapperProfile()
        {
            CreateMap<SessionCreateContract, SessionCreateDTO>().ReverseMap();
            CreateMap<SessionUpdateContract, SessionUpdateDTO>().ReverseMap();
            CreateMap<SessionParamsContract, SessionParamsDTO>().ReverseMap();
        }
    }
}
