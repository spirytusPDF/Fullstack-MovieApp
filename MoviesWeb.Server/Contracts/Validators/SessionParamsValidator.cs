using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class SessionParamsValidator : AbstractValidator<SessionParamsContract>
    {
        public SessionParamsValidator() 
        {

            RuleFor(s => s.StartsAt).NotEmpty().ExclusiveBetween(DateTime.Now, DateTime.Now.AddYears(1))
                .WithErrorCode("The session can start only in the future year");
            RuleFor(s => s.HallName).NotEmpty().MinimumLength(1);
            RuleFor(s => s.TicketPrice).NotEmpty().GreaterThan(0);
            RuleFor(s => s.AvailableSeats).NotEmpty().GreaterThan(0);
            RuleFor(m => m.MovieTitle).MinimumLength(1);
            RuleFor(m => m.Sort).MinimumLength(3);

            RuleFor(m => m.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
            RuleFor(m => m.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
        }
    }
}
