using FluentValidation;

namespace MoviesWeb.Server.Contracts.Validators
{
    public class AddCommentValidator: AbstractValidator<AddCommentContract>
    {
        public AddCommentValidator() {
            RuleFor(c => c.Title).NotEmpty().MinimumLength(1).MaximumLength(50);
            RuleFor(c => c.Text).NotEmpty().MinimumLength(2).MaximumLength(250);
        }
    }
}
