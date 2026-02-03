using FirstAPI.Models;
using FluentValidation;

namespace FirstAPI.Validation
{
    public class BookValidator: AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.Title)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(b=>b.Author)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(b => b.YearPublished)
                .InclusiveBetween(1500, DateTime.Now.Year);
        }
    }
}
