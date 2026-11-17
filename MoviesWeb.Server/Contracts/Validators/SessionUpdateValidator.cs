using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class SessionUpdateValidator: AbstractValidator<SessionUpdateContract>
    {
        public SessionUpdateValidator()
        {
            RuleFor(s => s.StartsAt).NotEmpty().ExclusiveBetween(DateTime.Now, DateTime.Now.AddYears(1))
                .WithErrorCode("The session can start only in the future year");
            RuleFor(s => s.HallName).NotEmpty().MinimumLength(1);
            RuleFor(s => s.TicketPrice).NotEmpty().GreaterThan(0);
        }
    }
}
