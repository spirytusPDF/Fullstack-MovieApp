using AutoMapper;
using MoviesWeb.Server.Contracts;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile() 
        {
            CreateMap<UserRegisterContract, UserRegisterDTO>().ReverseMap();
            CreateMap<UserLoginContract, UserLoginDTO>().ReverseMap();
        }
    }
}
