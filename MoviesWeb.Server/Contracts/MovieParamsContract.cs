namespace MoviesWeb.Server.Contracts
{
    public class MovieParamsContract
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public string? ReleaseYear { get; set; }
        public string? Description { get; set; }
        public string? Key { get; set; }
        public double? Rating { get; set; }
        public string? Sort { get; set; }

        public int? Page {  get; set; }
        public int? PageSize { get; set; }
    }
}
