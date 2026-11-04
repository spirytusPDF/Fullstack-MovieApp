using AutoMapper;
using Domain;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile() 
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserResponseDTO>().ReverseMap();
        }
    }
}
