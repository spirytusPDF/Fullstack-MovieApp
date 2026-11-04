using AutoMapper;
using Domain;
using Services.DTOs.Comments;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class CommentMapperProfile: Profile
    {
        public CommentMapperProfile()
        {
            CreateMap<MovieComment, CommentCreateDTO>().ReverseMap();
        }
    }
}
