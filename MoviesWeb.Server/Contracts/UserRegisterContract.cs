namespace MoviesWeb.Server.Contracts
{
    public class UserRegisterContract
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string? City { get; set; }
        public string? FavouriteGenre { get; set; }
    }
}
