using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.User
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
