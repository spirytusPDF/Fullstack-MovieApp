using AutoMapper;
using Domain;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class UserMapperProfileService: Profile
    {
        public UserMapperProfileService() 
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserResponseDTO>().ReverseMap();
        }
    }
}
