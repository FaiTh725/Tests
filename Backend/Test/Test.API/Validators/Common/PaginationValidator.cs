using FluentValidation;
using Test.API.Contracts.Common;

namespace Test.API.Validators.Common
{
    public class PaginationValidator : 
        AbstractValidator<GetPaginatedDataRequest>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.Page)
                .Must(x => x > 0)
                    .WithMessage("Page cant be less than zero");

            RuleFor(x => x.PageSize)
                .Must(x => x > 0)
                    .WithMessage("Page size cant be less than zero");
        }
    }
}
