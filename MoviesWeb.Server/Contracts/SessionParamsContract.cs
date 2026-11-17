namespace MoviesWeb.Server.Contracts
{
    public class SessionParamsContract
    {
        public DateTime? StartsAt { get; set; }
        public string? HallName { get; set; }
        public decimal? TicketPrice { get; set; }
        public int? AvailableSeats { get; set; }
        public string? MovieTitle { get; set; }
        public string? Sort { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
