namespace MoviesWeb.Server.Contracts
{
    public class UpdateMovieContract
    {
        public string Description { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
        public string? MovieUrl { get; set; }
    }
}
