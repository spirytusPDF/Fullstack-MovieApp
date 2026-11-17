namespace MoviesWeb.Server.Contracts
{
    public class SessionUpdateContract
    {
        public DateTime StartsAt { get; set; }
        public string HallName { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
