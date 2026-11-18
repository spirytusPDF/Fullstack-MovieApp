using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateContract>
    {
        public UserUpdateValidator() 
        {
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
            RuleFor(u => u.Password).NotEmpty().MinimumLength(6).MaximumLength(30)
                .Matches("[A-Z]").WithMessage("Password must have at least one capital letter.")
                .Matches("[a-z]").WithMessage("Password must have at least one small letter.")
                .Matches("[0-9]").WithMessage("Password must have at least one number.")
                .Matches("[#?!@$%^&*-]").WithMessage("Password must have at least one special symbol.");
            RuleFor(u=>u.NewPassword).MinimumLength(6).MaximumLength(30)
                .Matches("[A-Z]").WithMessage("Password must have at least one capital letter.")
                .Matches("[a-z]").WithMessage("Password must have at least one small letter.")
                .Matches("[0-9]").WithMessage("Password must have at least one number.")
                .Matches("[#?!@$%^&*-]").WithMessage("Password must have at least one special symbol.");

            RuleFor(u => u.NewGenre).MinimumLength(2).MaximumLength(70);
            RuleFor(u => u.NewLogin).MinimumLength(2).MaximumLength(70);
            RuleFor(u => u.NewCity).MinimumLength(2).MaximumLength(70);
        }

    }
}
