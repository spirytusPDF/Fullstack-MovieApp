using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Queries
{
    public class SessionParameters
    {
        public DateTime? StartsAt { get; set; }
        public string? HallName { get; set; }
        public decimal? TicketPrice { get; set; }
        public int? AvailableSeats { get; set; }
        public string? MovieTitle { get; set; }
        public string? Sort { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
