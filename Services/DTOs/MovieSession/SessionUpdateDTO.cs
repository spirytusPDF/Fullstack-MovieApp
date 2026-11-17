using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.MovieSession
{
    public class SessionUpdateDTO
    {
        public int Id { get; set; }
        public DateTime StartsAt { get; set; }
        public string HallName { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
