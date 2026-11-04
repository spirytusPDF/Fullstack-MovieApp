using Domain;
using Services.DTOs.Comments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Movie
{
    public class MovieResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
        public string? MovieUrl { get; set; }

        public List<CommentResponseDTO>? MovieComments { get; set; }
    }
}
