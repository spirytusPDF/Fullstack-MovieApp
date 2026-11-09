using AutoMapper;
using MoviesWeb.Server.Contracts;
using Services.DTOs.Comments;
using Services.DTOs.User;

namespace MoviesWeb.Server.MapperProfile
{
    public class CommentMapperProfile: Profile
    {
        public CommentMapperProfile()
        {
            CreateMap<AddCommentContract, CommentCreateDTO>().ReverseMap();
        }
    }
}
