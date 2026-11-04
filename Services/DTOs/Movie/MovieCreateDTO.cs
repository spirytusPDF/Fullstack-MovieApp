using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Movie
{
    public class MovieCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
        public string? MovieUrl { get; set; }
    }
}
