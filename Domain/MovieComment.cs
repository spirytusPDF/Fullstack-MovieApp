using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieComment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Posted {  get; set; }

        public int UserId { get; set; }
        public User user { get; set; }

        public int MovieId { get; set; }
        public Movie movie { get; set; }
    }
}
