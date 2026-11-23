using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class AuthResult
    {
        public bool Succes { get; set; }
        public string? UserId {  get; set; }
        public string? Token {  get; set; }
        public DateTime? ExpirationTime { get; set; }

        public string[] Errors { get; set; } = new string[0];
    }
}
