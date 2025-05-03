using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Contacts.FeedbackReport;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReportEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackReportEntity.GetReportWithReviewer
{
    public class GetReportWithReviewerHandler :
        IRequestHandler<GetReportWithReviewerQuery, FeedbackReportWithReviewer>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetReportWithReviewerHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<FeedbackReportWithReviewer> Handle(
            GetReportWithReviewerQuery request, 
            CancellationToken cancellationToken)
        {
            var report = await unitOfWork.ReportRepository
                .GetFeedbackReportByCriteria(
                    new ReportWithReviewerSpecification(request.Id), 
                    cancellationToken);
        
            if(report is null)
            {
                throw new NotFoundException("Feedback report doesnt exist");
            }

            return new FeedbackReportWithReviewer
            {
                Id = report.Id,
                IsApproval = report.IsApproval,
                CreatedTime = report.CreatedTime,
                FeedbackId = report.ReportedFeedbackId,
                ReportMessage = report.ReportMessage,
                Reviewer = new BaseProfileResponse
                {
                    Id = report.Reviewer.Id,
                    Email = report.Reviewer.Email,
                    Name = report.Reviewer.Name,
                }
            };
        }
    }
}
