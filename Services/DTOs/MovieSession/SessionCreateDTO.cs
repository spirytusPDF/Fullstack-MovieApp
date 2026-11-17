using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.MovieSession
{
    public class SessionCreateDTO
    {
        public DateTime StartsAt { get; set; }
        public string HallName { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableSeats { get; set; }

        public int MovieId { get; set; }
    }
}
