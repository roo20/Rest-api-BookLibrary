using FluentValidation;
using RESTful_api.Dtos;

namespace RESTful_api.Validators
{
    public class BookValidator : AbstractValidator<BookCreateDto>
    {
        public BookValidator()
        {
            RuleFor(b => b.Title).NotNull().Length(0, 50);
            RuleFor(b => b.Author).NotNull().Length(0, 50);
            RuleFor(b => b.Genre).NotNull().Length(0, 50);
            RuleFor(b => b.Description).NotNull().Length(0, 200);
            RuleFor(b => b.Price).NotEmpty().GreaterThan(0);
            RuleFor(b => b.PublishDate).NotNull();
        }
    }
}
