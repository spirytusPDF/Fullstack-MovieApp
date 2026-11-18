using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.User
{
    public class UserRegisterDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string? City { get; set; }
        public string? FavouriteGenre { get; set; }
    }
}
