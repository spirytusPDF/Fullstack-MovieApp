using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.User
{
    public class UserLoginDTO
    {
        public string Login {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
