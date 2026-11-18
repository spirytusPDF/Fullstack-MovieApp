namespace MoviesWeb.Server.Contracts
{
    public class UserUpdateContract
    {
        public string Email {  get; set; }
        public string Password { get; set; }
        public string? NewPassword {  get; set; }
        public string? NewGenre {  get; set; }
        public string? NewLogin { get; set; }
        public string? NewCity {  get; set; }
    }
}
