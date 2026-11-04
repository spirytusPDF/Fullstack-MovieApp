using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class MovieAddValidator: AbstractValidator<AddMovieContract>
    {
        public MovieAddValidator()
        {
           RuleFor(m=>m.Title).NotEmpty().MaximumLength(100);
           RuleFor(m => m.Description).NotEmpty().MinimumLength(5).MaximumLength(400);
           RuleFor(m => m.Genre).NotEmpty().MinimumLength(2).MaximumLength(30);

           RuleFor(m => m.ReleaseYear).NotEmpty().InclusiveBetween(1890, DateTime.Now.Year + 1);
           RuleFor(m => m.Rating).NotEmpty().InclusiveBetween(0.1, 10);

           RuleFor(m => m.PosterUrl).Matches("https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)");
           RuleFor(m => m.MovieUrl).Matches("https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)");
        }
    }
}
