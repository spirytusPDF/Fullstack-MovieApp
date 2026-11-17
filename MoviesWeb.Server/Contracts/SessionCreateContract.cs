namespace MoviesWeb.Server.Contracts
{
    public class SessionCreateContract
    {
        public DateTime StartsAt { get; set; }
        public string HallName { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableSeats { get; set; }

    }
}
