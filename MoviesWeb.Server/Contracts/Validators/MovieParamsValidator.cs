using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class MovieParamsValidator: AbstractValidator<MovieParamsContract>
    {
        public MovieParamsValidator() {
            RuleFor(m=>m.Id).GreaterThanOrEqualTo(1);
            RuleFor(m => m.Title).MinimumLength(2);
            RuleFor(m => m.Genre).MinimumLength(2);
            RuleFor(m => m.ReleaseYear).MinimumLength(3);
            RuleFor(m => m.Description).MinimumLength(3);
            RuleFor(m => m.Rating).GreaterThanOrEqualTo(0);
            RuleFor(m => m.Key).MinimumLength(1);
            RuleFor(m => m.Sort).MinimumLength(3);

            RuleFor(m => m.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
            RuleFor(m => m.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
        }
    }
}
