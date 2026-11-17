using AutoMapper;
using Domain;
using Repositories.Queries;
using Services.DTOs.Movie;
using Services.DTOs.MovieSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MapperProfile
{
    public class SessionMapperProfileService:Profile
    {
        public SessionMapperProfileService()
        {
            CreateMap<MovieSession, SessionCreateDTO>().ReverseMap();
            CreateMap<MovieSession, SessionUpdateDTO>().ReverseMap();
            CreateMap<MovieSession, SessionResponseDTO>().ReverseMap();
            CreateMap<SessionParameters, SessionParamsDTO>().ReverseMap();
        }
    }
}
