using FluentValidation;
using TestRating.Application.Contacts.Pagination;

namespace TestRating.API.Validators.Common
{
    public class PaginationValidator<T> : 
        AbstractValidator<BasePaginationResponse<T>>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.Page)
                .Must(x => x > 0)
                    .WithMessage("Page less then one");

            RuleFor(x => x.PageCount)
                .Must(x => x > 0)
                    .WithMessage("Page size less then one");
        }
    }
}
