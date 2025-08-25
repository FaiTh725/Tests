using FluentValidation;
using Test.API.Contracts.Group;

namespace Test.API.Validators.GroupValidators
{
    public class GetGroupTestsValidator : 
        AbstractValidator<GetGroupTestsRequest>
    {
        public GetGroupTestsValidator()
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
