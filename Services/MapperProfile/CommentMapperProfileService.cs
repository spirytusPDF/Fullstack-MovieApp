using AutoMapper;
using Domain;
using Services.DTOs.Comments;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class CommentMapperProfileService: Profile
    {
        public CommentMapperProfileService()
        {
            CreateMap<MovieComment, CommentCreateDTO>().ReverseMap();
            CreateMap<MovieComment, CommentResponseDTO>()
                .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.movie.Title))
                .ForMember(dest => dest.UserLogin, opt => opt.MapFrom(src => src.user.Login))
                .ReverseMap();
        }
    }
}
