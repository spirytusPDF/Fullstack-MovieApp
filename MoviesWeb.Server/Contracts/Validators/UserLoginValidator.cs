using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class UserLoginValidator:AbstractValidator<UserLoginContract>
    {
        public UserLoginValidator() 
        {
            RuleFor(u => u.Login).NotEmpty()
                .MinimumLength(2).MaximumLength(30);

            RuleFor(u => u.Password).NotEmpty()
                .MinimumLength(6).MaximumLength(30)
                .Matches("[A-Z]").WithMessage("Password must have at least one capital letter.")
                .Matches("[a-z]").WithMessage("Password must have at least one small letter.")
                .Matches("[0-9]").WithMessage("Password must have at least one number.")
                .Matches("[#?!@$%^&*-]").WithMessage("Password must have at least one special symbol.");

            RuleFor(u => u.Email).NotEmpty()
                .MinimumLength(2).MaximumLength(40).EmailAddress();
        }
    }
}
