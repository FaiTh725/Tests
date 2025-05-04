using FluentValidation;
using TestRating.API.Contracts.FeedbackReport;
using TestRating.Domain.Validators;

namespace TestRating.API.Validators.ReportFeedbackValidators
{
    public class SendReportValidator :
        AbstractValidator<SendReportRequest>
    {
        public SendReportValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .MinimumLength(ReportValidator.MIN_REPORT_MESSAGE_LENGTH)
                    .WithMessage("Minimum size of the message is " +
                        ReportValidator.MIN_REPORT_MESSAGE_LENGTH.ToString())
                .MaximumLength(ReportValidator.MAX_REPORT_MESSAGE_LENGTH)
                    .WithMessage("Maximum size of the message is " +
                        ReportValidator.MAX_REPORT_MESSAGE_LENGTH.ToString());
        }
    }
}
