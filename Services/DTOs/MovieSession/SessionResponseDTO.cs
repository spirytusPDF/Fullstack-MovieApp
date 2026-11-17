using Repositories.MovieRepo;
using Services.DTOs.Movie;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.MovieSession
{
    public class SessionResponseDTO
    {
        public int Id { get; set; }
        public DateTime StartsAt { get; set; }
        public string HallName { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableSeats { get; set; }

        public MovieResponseDTO Movie { get; set; }
    }
}
