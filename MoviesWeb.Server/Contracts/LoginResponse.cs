using System.Globalization;

namespace MoviesWeb.Server.Contracts
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
