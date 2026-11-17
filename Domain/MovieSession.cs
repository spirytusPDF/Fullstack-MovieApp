using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieSession
    {
        public int Id { get; set; }
        public DateTime StartsAt {  get; set; }
        public string HallName {  get; set; }
        public decimal TicketPrice {  get; set; }
        public int AvailableSeats {  get; set; }

        public int MovieId {  get; set; }
        public Movie Movie { get; set; }
    }
}
