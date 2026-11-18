using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.User
{
    public class UserUpdateDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? NewPassword { get; set; }
        public string? NewGenre { get; set; }
        public string? NewLogin { get; set; }
        public string? NewCity { get; set; }
    }
}
