using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User: IdentityUser<Guid>
    {
        public string Login {  get; set; }
        public string Surname { get; set; }
        public string? City {  get; set; }
        public string? FavouriteGenre {  get; set; }
        public List<MovieComment>? MovieComments { get; set; }

    }
}
